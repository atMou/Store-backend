using Basket.Application.Features.Cart.DeleteCouponFromCart;

namespace Basket.Presentation.Requests;

public record DeleteCouponFromCartRequest
{
    public Guid CouponId { get; init; }
    public Guid CartId { get; init; }
    public DeleteCouponFromCartCommand ToCommand()
    {
        return new DeleteCouponFromCartCommand(Shared.Domain.ValueObjects.CartId.From(CartId), Shared.Domain.ValueObjects.CouponId.From(CouponId));
    }
}   