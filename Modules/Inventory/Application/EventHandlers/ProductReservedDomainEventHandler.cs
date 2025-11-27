using Inventory.Domain.Events;

using MassTransit;

using MediatR;

using Shared.Application.Features.Inventory.Events;

namespace Inventory.Application.EventHandlers;

internal class ProductReservedDomainEventHandler(IPublishEndpoint endpoint) : INotificationHandler<ProductReservedDomainEvent>
{
    public async Task Handle(ProductReservedDomainEvent notification, CancellationToken cancellationToken)
    {

        await endpoint.Publish(new ProductReservedIntegrationEvent(notification.ProductId, notification.Qty, notification.StockLevel),
            cancellationToken);
    }
}