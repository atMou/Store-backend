using Basket.Application.Features.Coupon.CreateCoupon;

namespace Basket.Presentation.Requests;

public record CreateCouponRequest
{
    public string Description { get; init; } = null!;
    public string DiscountType { get; init; } = null!;
    public decimal DiscountValue { get; init; }
    public decimal? MinimumPurchaseAmount { get; init; }
    public DateTime ExpiryDate { get; init; }

    public CreateCouponCommand ToCommand()
    {
        return new CreateCouponCommand
        {
            Description = Description,
            DiscountType = DiscountType,
            DiscountValue = DiscountValue,
            MinimumPurchaseAmount = MinimumPurchaseAmount,
            ExpiryDate = ExpiryDate
        };
    }
}