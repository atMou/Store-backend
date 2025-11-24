namespace Basket.Application.Features.Cart.DeleteCartItem;

public record DeleteLineItemCommand(ProductId ProductId, CartId CartId) : ICommand<Fin<Unit>>;

internal class DeleteLineItemHandler(
    IUserContext userContext,
    BasketDbContext dbContext)
    : ICommandHandler<DeleteLineItemCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(DeleteLineItemCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from cart in SoftDeleteEntity<BasketDbContext, Domain.Models.Cart>(
                cart => cart.Id == command.CartId,
                cart => cart.DeleteLineItem(command.ProductId),
                NotFoundError.New($"Cart with Id {command.CartId.Value} not found."))
            from x in userContext.IsSameUser<IO>(cart.UserId,
                          UnAuthorizedError.New("You are not authorized to delete this cart item."))
                      | userContext.IsInRole<IO>(Role.Admin,
                          UnAuthorizedError.New("You are not authorized to delete this cart item.")).As()
            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

}