using MassTransit;
using Microsoft.Extensions.Logging;
using Order.Persistence;
using Shared.Application.Features.Payment.Events;
using Shared.Infrastructure.Logging;

namespace Order.Application.EventHandlers;

public class PaymentCancelledIntegrationEventHandler(
    OrderDBContext dbContext,
    ILogger<PaymentCancelledIntegrationEventHandler> logger,
    IClock clock
) : IConsumer<PaymentCancelledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentCancelledIntegrationEvent> context)
    {
        var message = context.Message;

        logger.LogInformation(
            LogEvents.OrderCancelled,
            "Processing payment cancelled event for Order {OrderId} - marking as payment failed",
            message.OrderId);

        var db = GetUpdateEntity<OrderDBContext, Domain.Models.Order>(
            order => order.Id == OrderId.From(message.OrderId),
            NotFoundError.New($"Order with ID {message.OrderId} not found"),
            null,
            o => o.MarkAsPaymentFailed(clock.UtcNow)
        ).Map(_ => unit);

        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        result.Match(
            _ => logger.LogInformation(
                "Successfully marked order {OrderId} as payment failed",
                message.OrderId),
            err => logger.LogError(
                "Failed to mark order {OrderId} as payment failed: {Error}",
                message.OrderId,
                err.Message));
    }
}
