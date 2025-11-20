using Basket.Application.Features.Coupon.ExpireCoupon;

namespace Basket.Presentation.Requests;

public record ExpireCouponRequest(Guid CouponId)
{
    public ExpireCouponCommand ToCommand()
    {
        return new ExpireCouponCommand(CouponId);
    }
}