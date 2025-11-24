namespace Shared.Application.Contracts.Carts.Results;
public record CartResult
{
    public Guid UserId { get; init; }
    public Guid CartId { get; init; }
    public decimal Total { get; init; }
    public decimal Tax { get; init; }
    public decimal TotalSub { get; init; }
    public decimal Discount { get; init; }
    public decimal TotalDiscounted { get; init; }

    public IEnumerable<CouponResult> Coupons { get; init; } = [];

    public IEnumerable<LineItemResult> LineItems { get; init; } = [];
}

