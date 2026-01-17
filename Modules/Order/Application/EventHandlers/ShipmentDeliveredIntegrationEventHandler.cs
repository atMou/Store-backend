using MassTransit;

using Microsoft.Extensions.Logging;

using Order.Persistence;

using Shared.Application.Features.Shipment.Events;
using Shared.Domain.Errors;
using Shared.Infrastructure.Hubs;
using Shared.Infrastructure.Hubs.Services;

namespace Order.Application.EventHandlers;

internal class ShipmentDeliveredIntegrationEventHandler(
    OrderDBContext dbContext,
    INotificationService notificationService,
    ILogger<ShipmentDeliveredIntegrationEventHandler> logger)
    : IConsumer<ShipmentDeliveredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ShipmentDeliveredIntegrationEvent> context)
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
                    // Update order status when shipment is delivered
                    var updateDb = GetUpdateEntity<OrderDBContext, Domain.Models.Order>(
                        o => o.Id == OrderId.From(message.OrderId),
                        NotFoundError.New($"Order with ID {message.OrderId} not found."),
                        null,
                        o => o.MarkAsDelivered(message.DeliveredAt)
                    ).Map(_ => unit);

                    var updateResult = await updateDb.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

                    updateResult.Match(
                        _ => logger.LogInformation($"Order {message.OrderId} marked as delivered"),
                        err => logger.LogError($"Error updating order {message.OrderId}: {err}")
                    );

                    // Send notification to user
                    var notification = new ShipmentStatusNotification
                    {
                        ShipmentId = message.ShipmentId,
                        OrderId = message.OrderId,
                        Status = "Delivered",
                        TrackingCode = "",
                        Message = "Your order has been delivered! Thank you for your purchase.",
                        UpdatedAt = message.DeliveredAt
                    };

                    await notificationService.NotifyShipmentStatusChanged(order.UserId.Value, notification);
                },
                err => Task.Run(() => logger.LogError($"Error retrieving order {message.OrderId}: {err}"))
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while processing shipment delivery for Order {message.OrderId}");
        }
    }
}
