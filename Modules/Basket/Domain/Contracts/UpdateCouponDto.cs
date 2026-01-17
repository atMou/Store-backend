namespace Basket.Domain.Contracts;

public record UpdateCouponDto
{
    public CouponId CouponId { get; init; }
    public string? Description { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public CouponStatus? Status { get; init; }
}
