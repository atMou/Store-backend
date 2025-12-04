using Product.Domain.Models;

namespace Product.Domain.Contracts;
public static class Extensions
{
    public static ProductResult ToResult(this Models.Product p)
    {
        return new ProductResult()
        {
            Id = p.Id.Value,
            Slug = p.Slug.Value,
            Brand = p.Brand.Name,
            Category = p.Category.Name,
            Discount = p.Discount?.Value,
            Price = p.Price.Value,
            NewPrice = p.NewPrice?.Value,
            Description = p.Description.Value,
            IsNew = p.Status.IsNew,
            IsFeatured = p.Status.IsFeatured,
            IsBestSeller = p.Status.IsBestSeller,
            IsTrending = p.Status.IsTrending,
            TotalReviews = p.TotalReviews,
            TotalSales = p.TotalSales,
            RatingValue = p.Rating.Value,
            RatingDescription = p.Rating.Description,
            Images = p.Images.Select(pi => pi.ToResult()).ToArray(),
            Variants = p.Variants.Select(v => v.ToResult()).ToArray(),
            Reviews = p.Reviews.Select(r => r.ToResult())
        };
    }

    public static IEnumerable<ProductResult> ToResult(this IEnumerable<Models.Product> ps)
    {
        return ps.Select(p => p.ToResult());
    }

    public static ProductVariantsResult ToResult(this Variant v)
    {
        return new ProductVariantsResult()
        {
            Id = v.Id.Value,
            Sku = v.Sku.Value,
            Size = v.Size.Name,
            Color = v.Color.Name,
            ColorHex = v.Color.Hex,
            StockLevel = v.StockLevel.ToString(),
            Images = v.Images.Select(pi => pi.ToResult()).ToArray(),

        };
    }

    public static ReviewResult ToResult(this Review review)
    {
        return new ReviewResult()
        {
            Id = review.Id.Value,
            UserId = review.UserId.Value,
            ProductId = review.ProductId.Value,
            Comment = review.Comment.Value,
            Rating = review.Rating.Value,
            RatingDescription = review.Rating.Description,
            CreatedAt = review.CreatedAt
        };
    }

    public static ImageResult ToResult(this ProductImage pi)
    {
        return new ImageResult()
        {
            Id = pi.Id.Value,
            Url = pi.ImageUrl.Value,
            AltText = pi.AltText,
            IsMain = pi.IsMain
        };
    }
    public static CategoryResult ToResult(this Category category)
    {
        return new CategoryResult
        {
            Name = category.Name,
            Subcategories = category.Subcategories.Select(sub => sub.ToResult()).ToList()
        };
    }

}
