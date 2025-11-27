using Basket.Application.Features.Cart.CheckOut;

namespace Basket.Presentation.Requests;

public record CartCheckoutRequest(Guid CartId)
{
    public CartCheckoutCommand ToCommand()
    {
        return new CartCheckoutCommand(
            Shared.Domain.ValueObjects.CartId.From(CartId)
        );
    }
}