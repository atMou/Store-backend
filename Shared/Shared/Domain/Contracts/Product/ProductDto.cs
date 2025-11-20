namespace Shared.Domain.Contracts.Product;
public record ProductDto
{
    public Guid Id { get; init; }
    public string Slug { get; init; } = null!;
    public string Sku { get; init; } = null!;
    public string Brand { get; init; } = null!;
    public string Size { get; init; } = null!;
    public decimal Price { get; init; }
    public string Category { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal? NewPrice { get; init; }
    public decimal? Discount { get; init; }
    public bool IsNew { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsTrending { get; init; }
    public bool IsBestSeller { get; init; }
    public int Stock { get; init; }
    //public string Currency { get; init; } = null!;
    public string ColorHex { get; init; } = null!;
    public string Color { get; init; } = null!;
    public double AverageRating { get; init; }
    public int TotalReviews { get; init; }
    public int TotalSales { get; init; }
    public string StockLevel { get; init; } = null!;


    public IEnumerable<ImageDto> Images { get; init; } = [];
    public IEnumerable<ProductVariantDto> Variants { get; init; } = [];
    public IEnumerable<ReviewDto> Reviews { get; init; } = [];
}