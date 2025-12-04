using MassTransit;

using Order.Domain.Events;

using Shared.Application.Features.Order.Events;

namespace Order.Application.EventHandlers;

public class OrderCreatedDomainEventHandler(IPublishEndpoint endpoint) : INotificationHandler<OrderCreatedDomainEvent>
{
    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await endpoint.Publish(new OrderCreatedIntegrationEvent()
        {
            UserId = notification.UserId,
            OrderId = notification.OrderId,
            OrderItemsDtos = notification.OrderItemsDtos,
            TotalAfterDiscounted = notification.TotalAfterDiscounted,
            Tax = notification.Tax,
            CartId = notification.CartId,
            Total = notification.Total,
            Subtotal = notification.Subtotal,
            Discount = notification.Discount,
            Address = notification.Address,
            CouponIds = notification.CouponIds,
            ShipmentCost = notification.ShipmentCost

        }, cancellationToken);
    }
}
