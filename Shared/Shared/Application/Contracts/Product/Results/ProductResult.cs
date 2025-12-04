namespace Shared.Application.Contracts.Product.Results;
public record ProductResult
{
    public Guid Id { get; init; }
    public string Slug { get; init; }
    public string Brand { get; init; }
    public decimal Price { get; init; }
    public string Category { get; init; }
    public string Description { get; init; }
    public decimal? NewPrice { get; init; }
    public decimal? Discount { get; init; }
    public bool IsNew { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsTrending { get; init; }
    public bool IsBestSeller { get; init; }
    public double RatingValue { get; init; }
    public string RatingDescription { get; init; }
    public int TotalReviews { get; init; }
    public int TotalSales { get; init; }


    public IEnumerable<string> Colors { get; init; } = [];
    public IEnumerable<string> Sizes { get; init; } = [];
    public IEnumerable<ProductVariantsResult> Variants { get; init; }
    public IEnumerable<ImageResult> Images { get; init; }
    public IEnumerable<ProductResult> SimilarProducts { get; init; }
    public IEnumerable<ReviewResult> Reviews { get; init; }
}