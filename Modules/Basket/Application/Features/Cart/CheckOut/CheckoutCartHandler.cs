using Basket.Domain.Events;

namespace Basket.Application.Features.Cart.CheckOut;

public record CartCheckoutCommand(CartId CartId) : ICommand<Fin<Unit>>;

internal class CheckoutCartCommandHandler(
    BasketDbContext dbContext,
    IUserContext userContext,
    IPublishEndpoint endpoint)
    : ICommandHandler<CartCheckoutCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(CartCheckoutCommand command, CancellationToken cancellationToken)
    {
        var db =
            from cart in GetEntity<BasketDbContext, Domain.Models.Cart>(
                cart => cart.Id == command.CartId,
                NotFoundError.New($"Cart with Id {command.CartId.Value} not found."),
                opt =>
                {
                    opt.AddInclude(cart => cart.LineItems);
                    return opt;
                })

            from a in userContext.IsSameUser<IO>(cart.UserId,
                UnAuthorizedError.New("You do not have permission to checkout this cart."))
            from _2 in when(cart.LineItems.Count == 0,
                Db<BasketDbContext>.fail<Unit>(InvalidOperationError.New("Cannot checkout an empty cart.")))

            from _3 in UpdateEntity<BasketDbContext, Domain.Models.Cart>(cart, cart => cart.SetCartCheckedOut())
            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnFail(async error => await endpoint.Publish(new CartCheckoutFailedDomainEvent(error), cancellationToken)
            );
    }
}