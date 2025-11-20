using Shared.Messaging.Events;

namespace Basket.Application.EventHandlers;

internal class CartCheckedOutDomainEventHandler(IPublishEndpoint endpoint) : INotificationHandler<CartCheckedOutDomainEvent>
{
    public async Task Handle(CartCheckedOutDomainEvent notification, CancellationToken cancellationToken)
    {
        await endpoint.Publish(new CartCheckedOutIntegrationEvent
        {
            UserId = notification.UserId,
            CartId = notification.CartId,
            Total = notification.Total,
            TotalSub = notification.TotalSub,
            Tax = notification.Tax,
            Discount = notification.Discount,
            TotalDiscounted = notification.TotalDiscounted,
            CouponIds = notification.CouponIds,
            LineItems = notification.LineItems
        }, cancellationToken);
    }
}
