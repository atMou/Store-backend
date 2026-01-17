using MassTransit;

using Order.Persistence;

using Shared.Application.Features.Payment.Events;
using Shared.Infrastructure.Hubs;
using Shared.Infrastructure.Hubs.Services;
using Shared.Infrastructure.Logging;

namespace Order.Application.EventHandlers;

public class PaymentFulfilledIntegrationEventHandler(
    OrderDBContext dbContext,
    INotificationService notificationService,
    ILogger<PaymentFulfilledIntegrationEventHandler> logger,
    IClock clock
) : IConsumer<PaymentFulfilledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentFulfilledIntegrationEvent> context)
    {
        var message = context.Message;

        logger.LogInformation(
            LogEvents.OrderCreated,
            "Processing payment fulfilled event for Order {OrderId} - marking as paid",
            message.OrderId);

        // Get order to access userId
        var getOrder = GetEntity<OrderDBContext, Domain.Models.Order>(
            order => order.Id == OrderId.From(message.OrderId),
            NotFoundError.New($"Order with ID {message.OrderId} not found"),
            null
        );

        var orderResult = await getOrder.RunAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        await orderResult.Match(
            async order =>
            {
                // Update order as paid
                var updateDb = GetUpdateEntity<OrderDBContext, Domain.Models.Order>(
                    o => o.Id == OrderId.From(message.OrderId),
                    NotFoundError.New($"Order with ID {message.OrderId} not found"),
                    null,
                    o => o.MarkAsPaid(PaymentId.From(message.PaymentId), clock.UtcNow)
                ).Map(_ => unit);

                var result = await updateDb.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

                result.Match(
                    _ => logger.LogInformation(
                        "Successfully marked order {OrderId} as paid",
                        message.OrderId),
                    err => logger.LogError(
                        "Error marking order {OrderId} as paid: {Error}",
                        message.OrderId,
                        err.Message));

                // Send payment success notification
                var paymentNotification = new PaymentStatusNotification
                {
                    OrderId = message.OrderId,
                    PaymentId = message.PaymentId,
                    Status = "Completed",
                    Message = $"Your payment has been successfully processed! Your order is being prepared for shipment.",
                    UpdatedAt = clock.UtcNow
                };

                await notificationService.NotifyPaymentUpdate(order.UserId.Value, paymentNotification);

                // Send order status update notification
                var orderNotification = new OrderStatusNotification
                {
                    OrderId = message.OrderId,
                    Status = "Processing",
                    Message = "Your order is being processed and will be shipped soon.",
                    UpdatedAt = clock.UtcNow
                };

                await notificationService.NotifyOrderStatusChanged(order.UserId.Value, orderNotification);
            },
            err =>
            {
                logger.LogError(
                    "Error retrieving order {OrderId}: {Error}",
                    message.OrderId,
                    err.ToString());
                return Task.CompletedTask;
            }
        );
    }
}
