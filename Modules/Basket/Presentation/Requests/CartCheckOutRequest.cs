using Basket.Application.Features.Cart.CheckOut;

namespace Basket.Presentation.Requests;

public record CartCheckOutRequest(Guid CartId)
{
    public CartCheckOutCommand ToCommand()
    {
        return new CartCheckOutCommand(
            Shared.Domain.ValueObjects.CartId.From(CartId)
        );
    }
}