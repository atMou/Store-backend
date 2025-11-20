namespace Shared.Domain.Contracts.Cart;
public record CartDto
{
    public Guid UserId { get; init; }
    public Guid CartId { get; init; }
    public decimal Total { get; init; }
    public decimal Tax { get; init; }
    public decimal TotalSub { get; init; }
    public decimal Discount { get; init; }
    public decimal TotalDiscounted { get; init; }

    public IEnumerable<CouponDto> Coupons { get; init; } = [];

    public IEnumerable<LineItemDto> LineItems { get; init; } = [];
}

