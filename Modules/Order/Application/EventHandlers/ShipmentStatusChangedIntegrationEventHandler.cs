using MassTransit;
using Microsoft.Extensions.Logging;
using Order.Persistence;
using Shared.Application.Features.Shipment.Events;
using Shared.Domain.Errors;
using Shared.Infrastructure.Hubs;
using Shared.Infrastructure.Hubs.Services;

namespace Order.Application.EventHandlers;

internal class ShipmentStatusChangedIntegrationEventHandler(
    OrderDBContext dbContext,
    INotificationService notificationService,
    ILogger<ShipmentStatusChangedIntegrationEventHandler> logger)
    : IConsumer<ShipmentStatusChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ShipmentStatusChangedIntegrationEvent> context)
    {
        var message = context.Message;

        try
        {
            logger.LogInformation(
                "Processing shipment status changed event for Shipment {ShipmentId}, Order {OrderId} - Status: {Status}",
                message.ShipmentId,
                message.OrderId,
                message.Status);

            // Get the order to access userId
            var getOrder = GetEntity<OrderDBContext, Domain.Models.Order>(
                order => order.Id == OrderId.From(message.OrderId),
                NotFoundError.New($"Order with ID {message.OrderId} not found."),
                null
            );

            var orderResult = await getOrder.RunAsync(dbContext, EnvIO.New(null, context.CancellationToken));

            orderResult.Match(
                order =>
                {
                    // Create appropriate notification message based on status
                    var notificationMessage = message.Status switch
                    {
                        "Pending" => "Your shipment is being prepared.",
                        "Shipped" => "Your order is on its way!",
                        "InTransit" => "Your package is in transit and will arrive soon.",
                        "OutForDelivery" => "Your package is out for delivery today!",
                        "Delivered" => "Your order has been delivered. Thank you for your purchase!",
                        "OnHold" => "Your shipment is currently on hold. We'll update you soon.",
                        "Cancelled" => "Your shipment has been cancelled.",
                        "Returned" => "Your shipment is being returned.",
                        _ => $"Your shipment status has been updated to: {message.Status}"
                    };

                    // Send notification to user
                    var notification = new ShipmentStatusNotification
                    {
                        ShipmentId = message.ShipmentId,
                        OrderId = message.OrderId,
                        Status = message.Status,
                        TrackingCode = order.TrackingCode?.Value ?? "",
                        Message = notificationMessage,
                        UpdatedAt = message.StatusChangedAt ?? DateTime.UtcNow
                    };

                    Try.lift(async () =>
                    {
                        await notificationService.NotifyShipmentStatusChanged(order.UserId.Value, notification);

                        logger.LogInformation(
                            "Shipment status notification sent to user {UserId} for Order {OrderId}",
                            order.UserId.Value,
                            message.OrderId);
                    }).Run().IfFail(ex =>
                    {
                        logger.LogError(ex,
                            "Error sending shipment status notification to user {UserId}",
                            order.UserId.Value);
                    });
                },
                err =>
                {
                    logger.LogError(
                        "Error retrieving order {OrderId}: {Error}",
                        message.OrderId,
                        err.ToString());
                }
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error occurred while processing shipment status change for Shipment {ShipmentId}",
                message.ShipmentId);
        }
    }
}
