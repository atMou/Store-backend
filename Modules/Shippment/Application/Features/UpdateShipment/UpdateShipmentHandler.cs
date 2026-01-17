using Shipment.Domain.Contracts;
using Shared.Application.Features.Shipment.Events;
using Shared.Domain.Errors;
using Shipment.Persistence;

namespace Shipment.Application.Features.UpdateShipment;

public record UpdateShipmentCommand : ICommand<Fin<Unit>>
{
    public ShipmentId ShipmentId { get; init; }
    public string? Street { get; init; }
    public string? City { get; init; }
    public uint? PostalCode { get; init; }
    public uint? HouseNumber { get; init; }
    public string? ExtraDetails { get; init; }
    public string? TrackingCode { get; init; }
    public string? Status { get; init; }
    public DateTime? StatusDate { get; init; }
}

internal class UpdateShipmentCommandHandler(
    ShipmentDbContext dbContext,
    IPublishEndpoint endpoint)
    : ICommandHandler<UpdateShipmentCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(
        UpdateShipmentCommand command,
        CancellationToken cancellationToken)
    {
        Address? address = null;
        if (!string.IsNullOrWhiteSpace(command.Street) &&
            !string.IsNullOrWhiteSpace(command.City))
        {
            address = new Address
            {
                Street = command.Street,
                City = command.City,
                PostalCode = command.PostalCode ?? 0,
                HouseNumber = command.HouseNumber ?? 0,
                ExtraDetails = command.ExtraDetails
            };
        }

        var dto = new UpdateShipmentDto
        {
            Address = address,
            TrackingCode = command.TrackingCode,
            Status = command.Status,
            StatusDate = command.StatusDate
        };

        // First get the entity before update to access its properties
        var getShipment = GetEntity<ShipmentDbContext, Domain.Models.Shipment>(
            shipment => shipment.Id == command.ShipmentId,
            NotFoundError.New($"Shipment with ID {command.ShipmentId.Value} not found."),
            null
        );

        var db = from shipment in getShipment
                 from updated in shipment.Update(dto)
                 from _ in Db<ShipmentDbContext>.lift(ctx =>
                 {
                     ctx.Set<Domain.Models.Shipment>().Update(updated);
                     return unit;
                 })
                 select updated;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async s =>
            {
                // Publish integration event if status changed
                if (!string.IsNullOrWhiteSpace(command.Status))
                {
                    var integrationEvent = new ShipmentStatusChangedIntegrationEvent(
                        s.Id.Value,
                        s.OrderId.Value,
                        command.Status,
                        command.StatusDate ?? DateTime.UtcNow
                    );
                    await endpoint.Publish(integrationEvent, cancellationToken);

                    // Publish delivery event if delivered
                    if (command.Status.Equals("delivered", StringComparison.OrdinalIgnoreCase) && s.DeliveredAt.HasValue)
                    {
                        var deliveryEvent = new ShipmentDeliveredIntegrationEvent(
                            s.Id.Value,
                            s.OrderId.Value,
                            s.DeliveredAt.Value
                        );
                        await endpoint.Publish(deliveryEvent, cancellationToken);
                    }
                }
                return unit;
            });
    }
}
