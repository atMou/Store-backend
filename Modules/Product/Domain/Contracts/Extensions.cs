using Product.Domain.Models;

namespace Product.Domain.Contracts;
public static class Extensions
{
    public static ProductDto ToDto(this Models.Product p)
    {
        return new ProductDto()
        {
            Id = p.Id.Value,
            Slug = p.Slug.Value,
            Sku = p.Sku.Value,
            IsNew = p.Status.IsNew,
            IsFeatured = p.Status.IsFeatured,
            IsBestSeller = p.Status.IsBestSeller,
            IsTrending = p.Status.IsTrending,
            Stock = p.Stock.Value,
            Price = p.Price.Value,
            Brand = p.Brand.Name,
            Size = p.Size.Name,
            Color = p.Color.Name,
            ColorHex = p.Color.Hex,
            Category = p.Category.Name,
            Description = p.Description.Value,
            NewPrice = p.NewPrice?.Value,
            Discount = p.Discount?.Value,
            AverageRating = p.AvgRating.Value,
            TotalReviews = p.TotalReviews,
            TotalSales = p.TotalSales,
            StockLevel = p.StockLevel.ToString(),
            Images = p.ProductImages.Select(pi => pi.ToImageDto()).ToArray(),
            Variants = p.Variants.Select(v => v.ToVariantDto()),
            Reviews = p.Reviews.Select(r => r.ToReviewsDto())
        };
    }

    public static IEnumerable<ProductDto> ToDto(this IEnumerable<Models.Product> ps)
    {
        return ps.Select(p => p.ToDto());
    }

    public static ProductVariantDto ToVariantDto(this Models.Product v)
    {
        return new ProductVariantDto()
        {
            Id = v.Id.Value,
            Sku = v.Sku.Value,
            Size = v.Size.Name,
            Color = v.Color.Name,
            ColorHex = v.Color.Hex,
            Price = v.Price.Value,
            Stock = v.Stock.Value
        };
    }

    public static ReviewDto ToReviewsDto(this Review review)
    {
        return new ReviewDto()
        {
            Id = review.Id.Value,
            UserId = review.UserId.Value,
            ProductId = review.ProductId.Value,
            Comment = review.Comment.Value,
            Rating = review.Rating.Value,
            RatingDescription = review.Rating.Description
        };
    }

    public static ImageDto ToImageDto(this ProductImage pi)
    {
        return new ImageDto()
        {
            Id = pi.Id.Value,
            Url = pi.ImageUrl.Value,
            AltText = pi.AltText,
            IsMain = pi.IsMain
        };
    }


}
