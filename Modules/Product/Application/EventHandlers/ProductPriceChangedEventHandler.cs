namespace Product.Application.EventHandlers;

//internal class ProductPriceChangedEventHandler(IBus bus, ILogger<ProductPriceChangedEventHandler> logger) : INotificationHandler<ProductPriceChangedEvent>
//{
//    public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
//    {

//        logger.LogInformation("ProductPriceChangedEvent is being called");

//        await bus.Publish(new ProductPriceChangedIntegrationEvent(notification.ProductId, notification.NewPrice), cancellationToken);

//    }
//}
