namespace Product.Domain.Contracts;

public record UpdateProductDto
{
    public Guid Id { get; init; }
    public string Slug { get; init; } = null!;
    public bool IsFeatured { get; init; }
    public bool IsNew { get; init; }
    public bool IsBestSeller { get; init; }
    public bool IsTrending { get; init; }
    public string[] ImageUrls { get; init; } = [];
    public int Stock { get; init; }
    public int LowStockThreshold { get; init; }
    public decimal Price { get; init; }
    public string Category { get; init; } = null!;
    public string Description { get; init; } = null!;
}
