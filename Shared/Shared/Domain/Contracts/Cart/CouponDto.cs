namespace Shared.Domain.Contracts.Cart;

public record CouponDto
{
    public Guid Id { get; init; }
    public Guid? CartId { get; init; }
    public Guid? UserId { get; init; }
    public string Code { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal DiscountValue { get; init; }
    public string DiscountType { get; init; } = null!;
    public decimal MinimumPurchaseAmount { get; init; }
    public DateTime ExpiryDate { get; init; }
    public string Status { get; init; }
    public bool IsDeleted { get; init; }
    public bool IsValid { get; init; }


}