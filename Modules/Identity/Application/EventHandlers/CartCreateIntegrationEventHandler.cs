namespace Identity.Application.EventHandlers;

public record CartCreateIntegrationEventHandler(IdentityDbContext DbContext) : IConsumer<CartCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CartCreatedIntegrationEvent> context)
    {
        var userId = context.Message.UserId;
        var cartId = context.Message.CartId;

        var db = from user in Db<IdentityDbContext>.liftIO(async ctx =>
                await ctx.Users.FirstOrDefaultAsync(user => user.Id == UserId.From(userId)))

                 let updatedUser = user.SetCartId(CartId.From(cartId))
                 from a in Db<IdentityDbContext>.lift(ctx =>
                 {
                     ctx.Users.Entry(user).CurrentValues.SetValues(updatedUser);
                     return unit;
                 })
                 select unit;

        Fin<Unit> result = await db.RunSaveAsync(DbContext, EnvIO.New(null, context.CancellationToken));
        result.Match(
            Succ: _ => { },
            Fail: err => throw new Exception($"Failed to update user with cart ID: {err}")
        );
    }
}
