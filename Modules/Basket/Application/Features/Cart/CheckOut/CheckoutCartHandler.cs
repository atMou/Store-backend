namespace Basket.Application.Features.Cart.CheckOut;

public record CartCheckOutCommand(CartId CartId) : ICommand<Fin<Unit>>;


internal class CheckoutCartCommandHandler(
    BasketDbContext dbContext,
    IUserContext userContext,
    IPublishEndpoint endpoint,
    ICartRepository cartRepository)
    : ICommandHandler<CartCheckOutCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(CartCheckOutCommand command, CancellationToken cancellationToken)
    {
        var db =
            from u in userContext.GetCurrentUser<IO>().As()

            from c in Db<BasketDbContext>.liftIO(ctx => cartRepository.GetCartById(command.CartId, ctx))
            from _1 in when(c.UserId.Value != u.Id,
                Db<BasketDbContext>.fail<Unit>(
                    UnAuthorizedError.New("You do not have permission to checkout this cart.")))
            from _2 in when(c.LineItems.Count == 0,
                Db<BasketDbContext>.fail<Unit>(InvalidOperationError.New("Cannot checkout an empty cart.")))
            from _3 in Db<BasketDbContext>.lift(c.SetCartCheckedOut)
            select unit;

        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnFail(
               async error => await endpoint.Publish(new CartCheckoutFailedEvent(error), cancellationToken)
            );
    }
}