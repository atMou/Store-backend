using Basket.Domain.Enums;

namespace Basket.Domain.Contracts;

public record UpdateCouponDto
{
    public Guid CouponId { get; init; }
    public string? Description { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public DiscountType? DiscountType { get; init; }
    public decimal? DiscountValue { get; init; }
    public decimal? MinimumPurchaseAmount { get; private init; }
    public CouponStatus? Status { get; private init; }

}