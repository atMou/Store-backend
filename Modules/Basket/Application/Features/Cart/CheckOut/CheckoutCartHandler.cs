using Microsoft.Extensions.Logging;

using Shared.Application.Features.Cart.Events;
using Shared.Infrastructure.Logging;

namespace Basket.Application.Features.Cart.CheckOut;

public record CartCheckoutCommand(CartId CartId) : ICommand<Fin<Unit>>;

internal class CheckoutCartCommandHandler(
    BasketDbContext dbContext,
    IUserContext userContext,
    IPublishEndpoint endpoint,
    ILogger<CheckoutCartCommandHandler> logger)
    : ICommandHandler<CartCheckoutCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(CartCheckoutCommand command, CancellationToken cancellationToken)
    {
        var db =
            from currentUser in userContext.GetCurrentUser<IO>().As()

            from cart in GetUpdateEntity<BasketDbContext, Domain.Models.Cart>(
                cart => cart.Id == command.CartId,
                NotFoundError.New($"Cart with Id {command.CartId.Value} not found."),
                opt =>
                {
                    opt = opt.AddInclude(cart => cart.LineItems);
                    return opt;
                },
                cart => cart.SetCartCheckedOut())

            from _1 in userContext.IsSameUser<IO>(cart.UserId,
                UnAuthorizedError.New("You do not have permission to checkout this cart.")).As()

            from _2 in when(cart.LineItems.Count == 0,
                Db<BasketDbContext>.fail<Unit>(InvalidOperationError.New("Cannot checkout an empty cart.")))

            from _3 in when(currentUser.HasPendingOrders,
                Db<BasketDbContext>.fail<Unit>(InvalidOperationError.New(
                    "You have a pending order. Please complete or cancel it before checking out again.")))

            select cart;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async cart =>
            {
                logger.LogCartCheckedOut(cart.Id.Value, cart.UserId.Value, cart.Total.Value);
                await endpoint.Publish(new CartCheckedOutIntegrationEvent()
                {
                    CartDto = cart.ToDto()
                }, cancellationToken);
                return unit;
            });

    }

}