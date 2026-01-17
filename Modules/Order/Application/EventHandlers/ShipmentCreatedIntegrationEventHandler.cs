using MassTransit;

using Order.Persistence;

using Shared.Application.Features.Shipment.Events;
using Shared.Infrastructure.Hubs;
using Shared.Infrastructure.Hubs.Services;

namespace Order.Application.EventHandlers;

internal class ShipmentCreatedIntegrationEventHandler(
    OrderDBContext dbContext,
    INotificationService notificationService,
    ILogger<ShipmentCreatedIntegrationEventHandler> logger)
    : IConsumer<ShipmentCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ShipmentCreatedIntegrationEvent> context)
    {
        var message = context.Message;

        try
        {
            // Get the order first to access userId
            var getOrder = GetEntity<OrderDBContext, Domain.Models.Order>(
                order => order.Id == OrderId.From(message.OrderId),
                NotFoundError.New($"Order with ID {message.OrderId} not found."),
                null
            );

            var orderResult = await getOrder.RunAsync(dbContext, EnvIO.New(null, context.CancellationToken));

            await orderResult.Match(
                async order =>
                {
                    // Update order with shipment ID and mark as shipped
                    var updateDb = GetUpdateEntity<OrderDBContext, Domain.Models.Order>(
                        o => o.Id == OrderId.From(message.OrderId),
                        NotFoundError.New($"Order with ID {message.OrderId} not found."),
                        null,
                        o => o.MarkAsShipped(
                            ShipmentId.From(message.ShipmentId),
                            DateTime.UtcNow)
                    ).Map(_ => unit);

                    var updateResult = await updateDb.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

                    updateResult.Match(
                        _ => logger.LogInformation($"Order {message.OrderId} marked as shipped with Shipment {message.ShipmentId}"),
                        err => logger.LogError($"Error updating order {message.OrderId}: {err}")
                    );

                    // Send notification to user
                    var notification = new ShipmentStatusNotification
                    {
                        ShipmentId = message.ShipmentId,
                        OrderId = message.OrderId,
                        Status = "Shipped",
                        TrackingCode = message.TrackingCode,
                        Message = $"Your order has been shipped! Tracking code: {message.TrackingCode}",
                        UpdatedAt = DateTime.UtcNow
                    };

                    await notificationService.NotifyShipmentStatusChanged(order.UserId.Value, notification);
                },
                err => Task.Run(() => logger.LogError($"Error retrieving order {message.OrderId}: {err}"))
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while processing shipment creation for Order {message.OrderId}");
        }
    }
}
