namespace Shared.Domain.Contracts.Cart;
public record CartDto
{
    public UserId UserId { get; init; } = null!;
    public CartId CartId { get; init; } = null!;
    public decimal Total { get; init; }
    public decimal TotalTax { get; init; }
    public decimal TotalAfterDiscount { get; init; }
    public decimal TotalDiscount { get; init; }

    public IEnumerable<CartLineItemDto> LineItems { get; init; } = [];
}
