using MassTransit;

using Microsoft.Extensions.Logging;

using Product.Domain.Events;

using Shared.Application.Features.Cart.Events;

namespace Product.Application.EventHandlers;

internal class ProductPriceChangedEventHandler(IPublishEndpoint publishEndpoint,
    ILogger<ProductPriceChangedEventHandler> logger)
    : INotificationHandler<ProductPriceChangedEvent>
{
    public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {

        logger.LogInformation("ProductPriceChangedEvent is being called");

        await publishEndpoint.Publish(
            new CartItemsPriceChangedIntegrationEvent(notification.Product.Id.Value,
                notification.Product.Price.Value, notification.NewPrice), cancellationToken);

    }
}
