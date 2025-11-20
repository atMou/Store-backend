namespace Shared.Domain.Contracts.Cart;
public record LineItemDto
{
    public Guid ProductId { get; init; }
    public string Slug { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}