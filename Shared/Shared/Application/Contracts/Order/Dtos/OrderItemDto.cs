namespace Shared.Application.Contracts.Order.Dtos;

public record OrderItemDto
{
    public Guid OrderItemId { get; init; }
    public Guid ProductId { get; init; }
    public Guid ColorVariantId { get; init; }
    public Guid SizeVariantId { get; init; }

    public string Sku { get; init; } = null!;
    public string Size { get; init; } = null!;
    public string Color { get; init; } = null!;

    public string Slug { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}