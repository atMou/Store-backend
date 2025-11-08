namespace Basket.Application.Features.Cart.DeleteCart;

public record DeleteCartCommand(CartId CartId) : ICommand<Fin<DeleteCartCommandResult>>;

public record DeleteCartCommandResult(bool isSuccessful);

internal class GetCartByIdCommandHandler(
    BasketDbContext dbContext,
    ICartRepository cartRepository,
    IUserContext userContext)
    : ICommandHandler<DeleteCartCommand, Fin<DeleteCartCommandResult>>
{
    public Task<Fin<DeleteCartCommandResult>> Handle(DeleteCartCommand command, CancellationToken cancellationToken)
    {
        var db = from c in Db<BasketDbContext>.liftIO(ctx =>
                cartRepository.DeleteCart(command.CartId, ctx))
                 from x in userContext.IsSameUser<IO>(c.UserId,
                                                               Error.New("You are not authorized to delete this cart item."))
                                                           | userContext.IsInRole<IO>(Role.Admin).As()
                 select new DeleteCartCommandResult(true);
        return db.RunSave(dbContext, EnvIO.New(null, cancellationToken));
    }
}