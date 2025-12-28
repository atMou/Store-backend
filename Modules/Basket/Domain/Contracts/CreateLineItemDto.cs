namespace Basket.Domain.Contracts;
public record CreateLineItemDto
{
	public ProductId ProductId { get; init; }
	public CartId CartId { get; init; } = null!;
	public string Slug { get; init; } = null!;
	public string Sku { get; init; } = null!;
	public string ImageUrl { get; init; } = null!;
	public int Quantity { get; init; }
	public decimal UnitPrice { get; init; }
}
