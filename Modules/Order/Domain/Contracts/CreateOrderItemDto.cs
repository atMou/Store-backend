namespace Order.Domain.Contracts;

public record CreateOrderItemDto
{
    public ProductId ProductId { get; init; }
    public ColorVariantId ColorVariantId { get; init; }
    public string Color { get; init; }
    public Guid SizeVariantId { get; init; }
    public string Size { get; init; }
    public string Sku { get; init; } = null!;
    public string Slug { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}