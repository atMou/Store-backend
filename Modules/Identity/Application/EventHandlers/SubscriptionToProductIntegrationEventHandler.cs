using Shared.Contracts.IntegrationEvents;

namespace Identity.Application.EventHandlers;

public class SubscriptionToProductIntegrationEventHandler(
    IdentityDbContext dbContext,
    ILogger<SubscriptionToProductIntegrationEventHandler> logger)
    : IConsumer<SubscriptionToProductIntegrationEvent>
{
    public async Task Consume(ConsumeContext<SubscriptionToProductIntegrationEvent> context)
    {
        var message = context.Message;
        var db = GetUpdateEntity<IdentityDbContext, User>(
            user => user.Id == UserId.From(message.UserId),
            NotFoundError.New($"User with ID {message.UserId} not found."),
            null,
            user => user.SubscribeToProduct(message.ProductId, message.ColorCode, message.SizeCode)
        );
        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        result.IfFail(error =>
        {
            logger.LogError(
                "Failed to update product subscription for user {UserId}: {Error}",
                message.UserId,
                error);
        });
    }
}