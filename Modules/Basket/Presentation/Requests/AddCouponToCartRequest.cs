using Basket.Application.Features.Cart.AddCouponToCart;

namespace Basket.Presentation.Requests;

public record AddCouponToCartRequest(Guid CartId, string CouponCode)
{
    public AddCouponToCartCommand ToCommand()
    {
        return new AddCouponToCartCommand(
            Shared.Domain.ValueObjects.CartId.From(CartId),
            CouponCode
        );
    }
}