using Basket.Application.Features.Coupon.CreateCoupon;
using Basket.Domain.Enums;

namespace Basket.Presentation.Requests;

public record CreateCouponRequest
{
    public string Description { get; init; } = null!;
    public DiscountType DiscountType { get; init; }
    public decimal DiscountValue { get; init; }
    public DateTime ExpiryDate { get; init; }


    public CreateCouponCommand ToCommand()
    {
        var dto = new CreateCouponDto
        {
            Description = Description,
            DiscountType = DiscountType,
            DiscountValue = DiscountValue,
            ExpiryDate = ExpiryDate,
        };
        return new CreateCouponCommand(dto);
    }
}