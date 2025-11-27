using Basket.Domain.Enums;

namespace Basket.Domain.Contracts;

public record UpdateCouponDto
{
    public CouponId CouponId { get; init; }
    public string? Description { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public DiscountType? DiscountType { get; init; }
    public decimal? DiscountValue { get; init; }
    public decimal? MinimumPurchaseAmount { get;  init; }
    public CouponStatus? Status { get;  init; }

}