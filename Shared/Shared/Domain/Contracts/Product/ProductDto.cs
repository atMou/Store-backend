namespace Shared.Domain.Contracts.Product;
public record ProductDto
{
    public Guid Id { get; init; }
    public string Slug { get; init; } = null!;
    public string Sku { get; init; } = null!;
    public bool IsFeatured { get; init; }
    public bool IsTrending { get; init; }
    public bool IsBestSeller { get; init; }
    public bool IsNew { get; init; }
    public string[] ImageUrls { get; init; } = [];
    public int Stock { get; init; }
    public int LowStockThreshold { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = null!;
    public string Brand { get; init; } = null!;
    public string Size { get; init; } = null!;
    public string ColorHex { get; init; } = null!;
    public string Color { get; init; } = null!;
    public string Category { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal? NewPrice { get; init; }
    public decimal? Discount { get; init; }
    public double AverageRating { get; init; }
    public int TotalReviews { get; init; }
    public int TotalSales { get; init; }
    public bool IsLowStock { get; init; }

    public IEnumerable<ProductVariantDto> Variants { get; init; } = [];
    public IEnumerable<ReviewDto> Reviews { get; init; } = [];
}