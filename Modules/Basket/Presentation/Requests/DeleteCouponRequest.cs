using Basket.Application.Features.Coupon.DeleteCoupon;

namespace Basket.Presentation.Requests;

public record DeleteCouponRequest
{
    public Guid CouponId { get; init; }
    public DeleteCouponCommand ToCommand() => new(Shared.Domain.ValueObjects.CouponId.From(CouponId));
}