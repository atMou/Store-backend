using Shared.Application.Features.Payment.Events;

namespace Identity.Application.EventHandlers;

public class PaymentFulfilledIntegrationEventHandler(
    IdentityDbContext dbContext,
    ILogger<PaymentFulfilledIntegrationEventHandler> logger)
    : IConsumer<PaymentFulfilledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentFulfilledIntegrationEvent> context)
    {
        var message = context.Message;
        var userId = UserId.From(message.UserId);

        logger.LogInformation(
            "Processing payment fulfilled event for Order {OrderId} - clearing pending order flag for user {UserId}",
            message.OrderId,
            message.UserId);

        var db = GetUpdateEntity<IdentityDbContext, User>(
            user => user.Id == userId,
            NotFoundError.New($"User with ID {message.UserId} not found"),
            opt => opt,
            user => user.SetHasPendingOrders(false)
        ).Map(_ => unit);

        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        result.Match(
            _ => logger.LogInformation(
                "Successfully cleared pending order flag for user {UserId}",
                message.UserId),
            err => logger.LogError(
                "Failed to clear pending order flag for user {UserId}: {Error}",
                message.UserId,
                err.Message));
    }
}
