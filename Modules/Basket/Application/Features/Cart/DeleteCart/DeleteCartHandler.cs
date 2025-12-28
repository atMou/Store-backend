namespace Basket.Application.Features.Cart.DeleteCart;

public record DeleteCartCommand(CartId CartId) : ICommand<Fin<Unit>>;


internal class DeleteCartByIdCommandHandler(
    BasketDbContext dbContext,
    IUserContext userContext)
    : ICommandHandler<DeleteCartCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(DeleteCartCommand command, CancellationToken cancellationToken)
    {
        var db = from c in HardDeleteEntity<BasketDbContext, Domain.Models.Cart>(
                     cart => cart.Id == command.CartId,
                     NotFoundError.New($"Cart with Id {command.CartId.Value} not found."))
                 from x in userContext.IsSameUser<IO>(c.UserId,
                                                               UnAuthorizedError.New("You are not authorized to delete this cart item."))
                                                           | userContext.HasRole<IO>(Role.Admin, UnAuthorizedError.New("You are not authorized to delete this cart item.")).As()
                 select unit;
        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}