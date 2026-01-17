using Shared.Application.Features.Shipment.Events;

using Shipment.Persistence;

namespace Shipment.Application.Features.CreateShipment;

public record CreateShipmentCommand : ICommand<Fin<Unit>>
{
    public OrderId OrderId { get; init; }
    public string TracingCode { get; set; }
    public string Street { get; init; }
    public string City { get; init; }
    public uint PostalCode { get; init; }
    public uint HouseNumber { get; init; }
    public string? ExtraDetails { get; init; }
}

internal class CreateShipmentCommandHandler(
    ShipmentDbContext dbContext,
    IPublishEndpoint endpoint)
    : ICommandHandler<CreateShipmentCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(
        CreateShipmentCommand command,
        CancellationToken cancellationToken)
    {
        var address = new Address
        {
            Street = command.Street,
            City = command.City,
            PostalCode = command.PostalCode,
            HouseNumber = command.HouseNumber,
            ExtraDetails = command.ExtraDetails
        };

        var shipment = Domain.Models.Shipment.Create(
            command.OrderId,
            address,
            command.TracingCode
        );

        var db = AddEntity<ShipmentDbContext, Domain.Models.Shipment>(shipment);

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async s =>
            {
                var integrationEvent = new ShipmentCreatedIntegrationEvent(
                    s.Id.Value,
                    s.OrderId.Value,
                    s.TrackingCode
                );
                await endpoint.Publish(integrationEvent, cancellationToken);
                return unit;
            });
    }

    private static string GenerateTrackingCode()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"TRK-{timestamp}-{random}";
    }
}
