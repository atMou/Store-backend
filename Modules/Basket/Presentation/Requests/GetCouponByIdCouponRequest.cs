using Basket.Application.Features.Coupon.GetCouponById;

namespace Basket.Presentation.Requests;

public record GetCouponByIdCouponRequest
{
    public Guid CouponId { get; init; }

    public GetCouponByIdCommand ToCommand()
    {
        return new GetCouponByIdCommand(CouponId);
    }
}