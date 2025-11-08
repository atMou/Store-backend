using Basket.Domain.Contracts;
using Shared.Domain.Contracts.Cart;
using Shared.Domain.Errors;

namespace Basket.Application.Features.Cart.CheckOut;

public record CartCheckOutCommand(CartId CartId) : ICommand<Fin<CheckOutCommandResult>>;

public record CheckOutCommandResult(CartDto CartDto);


internal class CheckoutCartCommandHandler(BasketDbContext dbContext, IUserContext userContext, IBus bus, ICartRepository cartRepository)
    : ICommandHandler<CartCheckOutCommand, Fin<CheckOutCommandResult>>
{
    public Task<Fin<CheckOutCommandResult>> Handle(CartCheckOutCommand command, CancellationToken cancellationToken)
    {
        var db =
            from u in userContext.GetCurrentUser<IO>().As()
            from c in Db<BasketDbContext>.liftIO( (ctx) => cartRepository.GetCartById(command.CartId,ctx  ))
            from _1 in when(c.UserId.Value != u.Id,
                Db<BasketDbContext>.fail<Unit>(UnAuthorizedError.New("You do not have permission to checkout this cart.")))
            from _2 in when(c.CartItems.Count == 0,
                Db<BasketDbContext>.fail<Unit>(InvalidOperationError.New("Cannot checkout an empty cart.")))
            from _3 in Db<BasketDbContext>.lift(() => c.SetCartCheckedOut(true))
            select new CheckOutCommandResult(c.ToDto());

        return db.RunSave(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseBiEvent(
                result => bus.Publish(new CartCheckedOutEvent(result.CartDto), cancellationToken),
                error => bus.Publish(new CartCheckoutFailedEvent(error), cancellationToken)
            );
    }

}

