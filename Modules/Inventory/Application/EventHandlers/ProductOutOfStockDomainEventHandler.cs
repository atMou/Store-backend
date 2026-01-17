using Inventory.Domain.Events;

using MediatR;

using Shared.Infrastructure.Hubs;
using Shared.Infrastructure.Hubs.Services;

namespace Inventory.Application.EventHandlers;

public class ProductOutOfStockDomainEventHandler(INotificationService notificationService, ILogger<ProductOutOfStockDomainEventHandler> logger) : INotificationHandler<ProductOutOfStockDomainEvent>
{
    public async Task Handle(ProductOutOfStockDomainEvent notification, CancellationToken cancellationToken)
    {
        var result = await (
            from _ in liftIO(async (e) =>
            {
                await notificationService.NotifyStockAlert(new StockAlertNotification()
                {
                    ProductId = notification.ProductId.Value,
                    Message = notification.Message,
                    Color = notification.Color,
                    Size = notification.Size,
                    Stock = notification.Stock,
                    Slug = notification.Slug,
                    IsAvailable = notification.IsAvailable
                });
                return unit;
            })
            select _
        ).RunSafeAsync(EnvIO.New(null, cancellationToken));


        result.IfFail(err =>
        {
            logger.LogError(err, $"Error sending out of stock notification for ProductId: {notification.ProductId}");
        });
    }
}
