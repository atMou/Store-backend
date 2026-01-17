using Basket.Application.Features.Coupon.UpdateCoupon;
using Basket.Domain.Contracts;

namespace Basket.Presentation.Requests;

public record UpdateCouponRequest
{
    public Guid CouponId { get; init; }
    public string? Description { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public string? Status { get; init; }

    public UpdateCouponCommand ToCommand()
    {
        var dto = new UpdateCouponDto
        {
            CouponId = Shared.Domain.ValueObjects.CouponId.From(CouponId),
            Description = Description,
            ExpiryDate = ExpiryDate,
            Status = Status is not null ? CouponStatus.FromUnsafe(Status) : null
        };

        return new UpdateCouponCommand(dto);
    }
}
