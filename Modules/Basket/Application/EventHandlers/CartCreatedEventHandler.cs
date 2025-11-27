using Basket.Domain.Events;

using Shared.Application.Features.Cart.Events;

namespace Basket.Application.EventHandlers;

public class CartCreatedEventHandler(IPublishEndpoint endpoint) : INotificationHandler<CartCreatedDomainEvent>
{
    public async Task Handle(CartCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await endpoint.Publish(new CartCreatedIntegrationEvent(notification.CartId.Value, notification.UserId.Value), cancellationToken);
    }


}
