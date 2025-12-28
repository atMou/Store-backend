namespace Basket.Application.Features.Cart.DeleteCartItem;

public record DeleteLineItemCommand(VariantId VariantId, CartId CartId) : ICommand<Fin<Unit>>;

internal class DeleteLineItemHandler(
    IUserContext userContext,
    BasketDbContext dbContext)
    : ICommandHandler<DeleteLineItemCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(DeleteLineItemCommand command,
        CancellationToken cancellationToken)
    {
        var db =

            from c in GetUpdateEntity<BasketDbContext, Domain.Models.Cart>(
                cart => cart.Id == command.CartId,
                NotFoundError.New($"Cart with Id {command.CartId.Value} not found."),
                opt =>
                {
                    opt.AsSplitQuery = true;
                    opt = opt.AddInclude(cart => cart.LineItems);
                    return opt;
                },
                cart => cart.DeleteLineItem(command.VariantId))
            from x in userContext.IsSameUser<IO>(c.UserId,
                          UnAuthorizedError.New("You are not authorized to delete this cart item."))
                      | userContext.HasRole<IO>(Role.Admin,
                          UnAuthorizedError.New("You are not authorized to delete this cart item.")).As()
            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

}