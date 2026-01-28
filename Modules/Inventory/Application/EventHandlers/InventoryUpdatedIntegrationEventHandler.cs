using Shared.Domain.Enums;
using Shared.Infrastructure.Hubs;
using Shared.Infrastructure.Hubs.Services;

namespace Inventory.Application.EventHandlers;

public class InventoryUpdatedIntegrationEventHandler(
    INotificationService notificationService,
    ILogger<InventoryUpdatedIntegrationEventHandler> logger)
    : IConsumer<InventoryUpdatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<InventoryUpdatedIntegrationEvent> context)
    {
        var message = context.Message;
        var notifications = GetNotifications(message);
        var result = notifications.AsIterable().Traverse(notification => liftIO(async e => await notificationService.NotifyStockAlert(notification))).As();
        var res = await result.RunSafeAsync(EnvIO.New(null, context.CancellationToken));
        res.Match(
            _ => logger.LogInformation("All stock alert notifications sent successfully for Product {ProductId} ({Slug})", message.ProductId, message.Slug),
            err =>
            {
                var exception = err.Exception.Match(ex => ex, () => null);
                logger.LogError(
                    exception,
                    "Error sending stock alert notifications for Product {ProductId} ({Slug}): {ErrorMessage}",
                    message.ProductId,
                    message.Slug,
                    err.Message
                );
            }
        );
    }


    private IEnumerable<StockAlertNotification> GetNotifications(
        InventoryUpdatedIntegrationEvent ev)
    {
        foreach (var cv in ev.ColorVariants)
        {
            foreach (var sv in cv.SizeVariants)
            {
                var stockAlertNotification = new StockAlertNotification
                {
                    ProductId = ev.ProductId,

                    Slug = ev.Slug,
                    Color = cv.Color,
                    Size = sv.Size,
                    Stock = sv.Stock,
                    IsAvailable = sv.Level > StockLevel.OutOfStock,
                    Brand = ev.Brand,
                    Message = sv.Level > StockLevel.OutOfStock ?
                        $"?? Good news! {ev.Brand}, {ev.Slug} with size {sv.Size} is back in stock!"
                        : $"?? Alert: {ev.Brand}, {ev.Slug} with size {sv.Size} is out of stock."
                };
                yield return stockAlertNotification;
            }
        }

    }
}
