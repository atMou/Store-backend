namespace Basket.Application.Features.Cart.DeleteCart;

public record DeleteCartCommand(CartId CartId) : ICommand<Fin<Unit>>;


internal class DeleteCartByIdCommandHandler(
    BasketDbContext dbContext,
    ICartRepository cartRepository,
    IUserContext userContext)
    : ICommandHandler<DeleteCartCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(DeleteCartCommand command, CancellationToken cancellationToken)
    {
        var db = from c in Db<BasketDbContext>.liftIO(ctx =>
                cartRepository.DeleteCart(command.CartId, ctx))
                 from x in userContext.IsSameUser<IO>(c.UserId,
                                                               UnAuthorizedError.New("You are not authorized to delete this cart item."))
                                                           | userContext.IsInRole<IO>(Role.Admin, UnAuthorizedError.New("You are not authorized to delete this cart item.")).As()
                 select unit;
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}