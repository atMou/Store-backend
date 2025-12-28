//using Inventory.Domain.Events;

//using MediatR;

//namespace Inventory.Application.EventHandlers;

//public class ProductReservedDomainEventHandler(IPublishEndpoint endpoint) : INotificationHandler<ProductReservedDomainEvent>
//{
//    public async Task Handle(ProductReservedDomainEvent notification, CancellationToken cancellationToken)
//    {

//        await endpoint.Publish(new ProductReservedIntegrationEvent(notification.ProductId, notification.Qty, notification.),
//            cancellationToken);
//    }
//}