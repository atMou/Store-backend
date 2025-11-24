using Basket.Application.Features.Cart.AddCouponToCart;

namespace Basket.Presentation.Requests;

public record AddCouponToCartRequest(Guid CartId, Guid CouponId)
{
    public AddCouponToCartCommand ToCommand()
    {
        return new AddCouponToCartCommand(
            Shared.Domain.ValueObjects.CartId.From(CartId),
            Shared.Domain.ValueObjects.CouponId.From(CouponId)
        );
    }
}