namespace Basket.Domain.Contracts;
public record CreateCartItemDto
{
    public Guid CartId { get; init; }
    public string Slug { get; init; } = null!;
    public string Sku { get; init; } = null!;
    public Guid ProductId { get; init; }
    public string ImageUrl { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
