using Shared.Application.Features.Order.Events;

namespace Identity.Application.EventHandlers;
internal class OrderCreatedIntegrationEventHandler(IdentityDbContext dbContext, ILogger<OrderCreatedIntegrationEventHandler> logger) : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var userId = UserId.From(context.Message.OrderDto.UserId);

        var db = GetUpdateEntity<IdentityDbContext, User>(
            u => u.Id == userId,
            NotFoundError.New($"User with id '{userId}' does not exist."),
            opt => opt,
            user => user.SetHasPendingOrders(true)
        );

        var results = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        results.IfFail(err => logger.LogCritical("Failed to set pending order flag for user {UserId}. {Error}", userId, err));
    }
}
