namespace Shared.Application.Contracts.Product.Results;
public record ProductResult
{
    public Guid Id { get; init; }
    public string Slug { get; init; }
    public string Sku { get; init; }
    public string Brand { get; init; }
    public string Size { get; init; }
    public decimal Price { get; init; }
    public string Category { get; init; }
    public string Description { get; init; }
    public decimal? NewPrice { get; init; }
    public decimal? Discount { get; init; }
    public bool IsNew { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsTrending { get; init; }
    public bool IsBestSeller { get; init; }
    public int Stock { get; init; }
    public string ColorHex { get; init; }
    public string Color { get; init; }
    public double RatingValue { get; init; }
    public string RatingDescription { get; init; }
    public int TotalReviews { get; init; }
    public int TotalSales { get; init; }
    public string Availability { get; init; }



    public IEnumerable<ImageResult> Images { get; init; }
    public IEnumerable<ProductVariantResult> Variants { get; init; }
    public IEnumerable<ReviewResult> Reviews { get; init; } = [];
}