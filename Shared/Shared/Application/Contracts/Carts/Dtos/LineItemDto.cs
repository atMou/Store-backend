namespace Shared.Application.Contracts.Carts.Dtos;

public record LineItemDto
{
    public Guid ProductId { get; init; }
    public Guid ColorVariantId { get; init; }
    public Guid SizeVariantId { get; init; }
    public string Sku { get; init; }
    public string Color { get; init; }
    public string Size { get; init; }
    public string Slug { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
