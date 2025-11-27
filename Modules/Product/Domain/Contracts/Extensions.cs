using Product.Domain.Models;

namespace Product.Domain.Contracts;
public static class Extensions
{
    public static ProductResult ToResult(this Models.Product p)
    {
        return new ProductResult()
        {
            Id = p.Id.Value,
            Sku = p.Sku.Value,
            Slug = p.Slug.Value,
            Brand = p.Brand.Name,
            Size = p.Size.Name,
            Color = p.Color.Name,
            ColorHex = p.Color.Hex,
            Category = p.Category.Name,
            Discount = p.Discount?.Value,
            Price = p.Price.Value,
            NewPrice = p.NewPrice?.Value,
            Description = p.Description.Value,
            IsNew = p.Status.IsNew,
            IsFeatured = p.Status.IsFeatured,
            IsBestSeller = p.Status.IsBestSeller,
            IsTrending = p.Status.IsTrending,
            Stock = p.Stock,
            Availability = p.StockLevel.ToString(),
            TotalReviews = p.TotalReviews,
            TotalSales = p.TotalSales,
            RatingValue = p.Rating.Value,
            RatingDescription = p.Rating.Description,
            Images = p.ProductImages.Select(pi => pi.ToResult()).ToArray(),
            Variants = p.Variants.Select(v => v.ToVariantsResult()),
            Reviews = p.Reviews.Select(r => r.ToResult())
        };
    }

    public static IEnumerable<ProductResult> ToResult(this IEnumerable<Models.Product> ps)
    {
        return ps.Select(p => p.ToResult());
    }

    public static ProductVariantResult ToVariantsResult(this Models.Product v)
    {
        return new ProductVariantResult()
        {
            Id = v.Id.Value,
            Sku = v.Sku.Value,
            Size = v.Size.Name,
            Color = v.Color.Name,
            ColorHex = v.Color.Hex,
            Price = v.Price.Value,
            Stock = v.Stock
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
            RatingDescription = review.Rating.Description
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


}
