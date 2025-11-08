namespace Basket.Application.Features.Cart.DeleteCartItem;

public record DeleteCartItemCommand(CartItemId CartItemId, CartId CartId) : ICommand<Fin<DeleteCartItemResult>>;

public record DeleteCartItemResult(bool isSuccessful);

internal class DeleteCartItemHandler(
    IUserContext userContext,
    BasketDbContext dbContext,
    ICartRepository cartRepository)
    : ICommandHandler<DeleteCartItemCommand, Fin<DeleteCartItemResult>>
{
    public async Task<Fin<DeleteCartItemResult>> Handle(DeleteCartItemCommand command,
        CancellationToken cancellationToken)
    {
        var db = from c in Db<BasketDbContext>.liftIO(ctx =>
                cartRepository.DeleteCartItem(command.CartItemId, command.CartId, ctx))
                 from x in Db<BasketDbContext>.liftIO(_ => userContext.IsSameUser<IO>(
                                                               c.UserId,
                                                               Error.New("You are not authorized to delete this cart item."))
                                                           | userContext.IsInRole<IO>(Role.Admin).As())
                 select new DeleteCartItemResult(true);


        return await db.RunSave(dbContext, EnvIO.New(null, cancellationToken));
    }
}