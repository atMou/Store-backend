using Basket.Domain.Enums;

namespace Basket.Domain.Contracts;

public record CreateCouponDto
{
	public string Description { get; init; } = null!;
	public DiscountType DiscountType { get; init; }
	public decimal DiscountValue { get; init; }
	public DateTime ExpiryDate { get; init; }
	public DateTime CurrentDate { get; init; }
	public decimal MinimumPurchaseAmount { get; init; }
}