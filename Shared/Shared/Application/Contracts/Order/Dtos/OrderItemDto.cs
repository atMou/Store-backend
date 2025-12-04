namespace Shared.Application.Contracts.Order.Dtos;

public record OrderItemDto
{
    public Guid ProductId { get; init; }
    public string Sku { get; init; } = null!;
    public string Slug { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}