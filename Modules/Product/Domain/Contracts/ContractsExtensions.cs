using Product.Domain.Models;

using Shared.Domain.Contracts.Product;

namespace Product.Domain.Contracts;
public static class ContractsExtensions
{
    public static ProductDto ToDto(this Models.Product p)
    {
        return new ProductDto()
        {
            Id = p.Id.Value,
            Slug = p.Slug.Value,
            Sku = p.Sku.Value,
            IsNew = p.ProductStatus.IsNew,
            IsFeatured = p.ProductStatus.IsFeatured,
            IsBestSeller = p.ProductStatus.IsBestSeller,
            IsTrending = p.ProductStatus.IsTrending,
            ImageUrls = p.ImageUrls.Select(url => url.Value).ToArray(),
            Stock = p.Stock,
            LowStockThreshold = p.LowStockThreshold,
            Price = p.Price.Value,
            Currency = p.Currency.Code,
            Brand = p.Brand.Name,
            Size = p.Size.Name,
            Color = p.Color.Name,
            ColorHex = p.Color.Hex,
            Category = p.Category.Name,
            Description = p.Description.Value,
            NewPrice = p.NewPrice.ValueUnsafe()?.Value,
            Discount = p.Discount.ValueUnsafe()?.Value,
            AverageRating = p.AvgRating.Value,
            TotalReviews = p.TotalReviews,
            TotalSales = p.TotalSales,
            IsLowStock = p.IsLowStock,
            Variants = p.Variants.Select(v => v.ToVariantDto()),
            Reviews = p.Reviews.Select(r => r.ReviewsDto())
        };
    }

    private static ProductVariantDto ToVariantDto(this Models.Product v)
    {
        return new ProductVariantDto()
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

    private static ReviewDto ReviewsDto(this Review review)
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


}
