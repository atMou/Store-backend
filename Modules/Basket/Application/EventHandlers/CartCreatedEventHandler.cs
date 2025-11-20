using Shared.Messaging.Events;

namespace Basket.Application.EventHandlers;

public class CartCreatedEventHandler(IPublishEndpoint endpoint) : INotificationHandler<CartCreatedEvent>
{
    public async Task Handle(CartCreatedEvent notification, CancellationToken cancellationToken)
    {
        await endpoint.Publish(new CartCreatedIntegrationEvent(notification.CartId.Value, notification.UserId.Value), cancellationToken);
    }


}
