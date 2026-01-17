using MassTransit;

using Order.Domain.Contracts;
using Order.Persistence;

using Shared.Application.Features.Order.Events;
using Shared.Domain.Errors;

namespace Order.Application.Features.UpdateOrder;

public record UpdateOrderCommand : ICommand<Fin<Unit>>
{
    public OrderId OrderId { get; init; }
    public string? Status { get; init; }
    public DateTime? StatusDate { get; init; }
    public Guid? PaymentId { get; init; }
    public Guid? ShipmentId { get; init; }
}

internal class UpdateOrderCommandHandler(
    OrderDBContext dbContext,
    IPublishEndpoint endpoint)
    : ICommandHandler<UpdateOrderCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(
        UpdateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var dto = new UpdateOrderDto
        {
            Status = command.Status,
            StatusDate = command.StatusDate,
            PaymentId = command.PaymentId,
            ShipmentId = command.ShipmentId
        };

        // First get the entity before update to access its properties
        var getOrder = GetEntity<OrderDBContext, Domain.Models.Order>(
            order => order.Id == command.OrderId,
            NotFoundError.New($"Order with ID {command.OrderId.Value} not found."),
            null
        );

        var db = from order in getOrder
                 from updated in order.Update(dto)
                 from _ in Db<OrderDBContext>.lift(ctx =>
                 {
                     ctx.Set<Domain.Models.Order>().Update(updated);
                     return unit;
                 })
                 select updated;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async o =>
            {
                // Publish integration event if status changed
                if (!string.IsNullOrWhiteSpace(command.Status))
                {
                    var integrationEvent = new OrderStatusChangedIntegrationEvent(
                        o.Id.Value,
                        command.Status,
                        command.StatusDate ?? DateTime.UtcNow
                    );
                    await endpoint.Publish(integrationEvent, cancellationToken);
                }
                return unit;
            });
    }
}
