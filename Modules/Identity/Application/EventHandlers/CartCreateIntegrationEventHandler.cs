using Shared.Application.Features.Cart.Events;

namespace Identity.Application.EventHandlers;

public record CartCreateIntegrationEventHandler(
    IdentityDbContext DbContext,
    ILogger<CartCreateIntegrationEventHandler> Logger)
    : IConsumer<CartCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CartCreatedIntegrationEvent> context)
    {
        Logger.LogInformation(
            "Processing CartCreatedIntegrationEvent for UserId: {UserId}, CartId: {CartId}",
            context.Message.UserId,
            context.Message.CartId);

        var userId = context.Message.UserId;
        var cartId = context.Message.CartId;

        var db = GetUpdateEntity<IdentityDbContext, User>(
            user => user.Id == UserId.From(userId),
            NotFoundError.New($"User with ID '{userId}' not found."),
            null,
            (user) => user.AddCartId(CartId.From(cartId))
        ).Map(_ => unit);

        var result = await db.RunSaveAsync(DbContext, EnvIO.New(null, context.CancellationToken));

        result.Match(
            Succ: _ => Logger.LogInformation(
                "Successfully updated User {UserId} with CartId {CartId}",
                userId,
                cartId),
            Fail: err => Logger.LogError(
                "Failed to update User {UserId} with CartId {CartId}. Error: {@Error}",
                userId,
                cartId,
                err)
        );
    }
}
