namespace Basket.Application.Features.Cart.DeleteCartItem;

public record DeleteLineItemCommand(ProductId ProductId, CartId CartId) : ICommand<Fin<Unit>>;

internal class DeleteLineItemHandler(
    IUserContext userContext,
    BasketDbContext dbContext,
    ICartRepository cartRepository)
    : ICommandHandler<DeleteLineItemCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(DeleteLineItemCommand command,
        CancellationToken cancellationToken)
    {
        var db = from c in Db<BasketDbContext>.liftIO(ctx =>
                cartRepository.DeleteLineItem(command.ProductId, command.CartId, ctx))
                 from x in Db<BasketDbContext>.liftIO(_ => userContext.IsSameUser<IO>(
                                                               c.UserId,
                                                               UnAuthorizedError.New("You are not authorized to delete this cart item."))
                                                           | userContext.IsInRole<IO>(Role.Admin, UnAuthorizedError.New("You are not authorized to delete this cart item.")).As())
                 select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}