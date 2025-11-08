namespace Shared.Domain.Contracts.Cart;
public record CartLineItemDto
{
    public ProductId ProductId { get; init; } = null!;
    public string Sku { get; init; } = null!;
    public string Slug { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}