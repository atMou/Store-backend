namespace Shared.Application.Contracts.Carts.Results;

public record CouponResult
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
    public string Status { get; init; } = null!;


}