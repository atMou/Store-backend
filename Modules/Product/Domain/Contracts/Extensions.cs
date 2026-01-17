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
            Category = p.Category.Main,
            SubCategory = p.Category.Sub,
            Discount = p.Discount?.Value,
            Price = p.Price.Value,
            NewPrice = p.NewPrice?.Value,
            Description = p.Description.Value,
            TotalReviews = p.TotalReviews,
            TotalSales = p.TotalSales,
            Rating = p.Rating.ToResult(),
            Status = p.Status.ToResult(),
            Images = p.Images.Select(pi => pi.ToResult()).ToArray(),
            ColorVariants = p.ColorVariants.Select(v => v.ToResult()).ToArray(),
            Reviews = p.Reviews.Select(r => r.ToResult()),
            Colors = p.Colors.Select(color => color.ToResult()),
            Sizes = p.Sizes.Select(size => size.ToResult()),
            Alternatives = p.Alternatives.Select(a => a.ToResult()),
            SizeFitAttributes = p.SizeFitAttributes.Select(attr => attr.ToResult()),
            DetailsAttributes = p.DetailsAttributes.Select(attr => attr.ToResult()),
            MaterialDetails = p.MaterialDetails.Select(md => md.ToResult()),
            ProductType = p.ProductType.ToResult()



        };
    }
    public static MaterialDetailResult ToResult(this MaterialDetail md)
    {
        return new MaterialDetailResult()
        {
            Material = md.Material.Name,
            Percentage = md.Percentage,
            Detail = md.Detail
        };

    }

    public static IEnumerable<ProductResult> ToResult(this IEnumerable<Models.Product> ps)
    {
        return ps.Select(p => p.ToResult());
    }

    public static ColorVariantsResult ToResult(this ColorVariant v)
    {
        return new ColorVariantsResult()
        {

            Id = v.Id.Value,
            Color = v.Color.ToResult(),
            Images = v.Images.Select(pi => pi.ToResult()).ToArray(),
            SizeVariants = v.SizeVariants.Select(sv => sv.ToResult()).ToArray()
        };
    }

    private static SizeVariantResult ToResult(this SizeVariant sv)
    {
        return new SizeVariantResult()
        {
            Id = sv.Id,
            Size = sv.Size.ToResult(),
            Stock = sv.Stock,
            StockLevel = sv.StockLevel.ToString(),
            Sku = sv.Sku.Value
        };
    }


    private static ReviewResult ToResult(this Review review)
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

    private static ImageResult ToResult(this Image pi)
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
            Main = category.Main,
            Sub = category.Sub,
            ProductTypes = category.ProductTypes.Select(pt => new ProductTypeCategoryResult
            {
                Type = pt.Type,
                SubTypes = pt.AllowedSubTypes
            }).ToArray(),
        };
    }

    public static ProductTypeResult ToResult(this ProductType productType)
    {
        return new ProductTypeResult
        {
            Type = productType.Type,
            SubType = productType.SubType,

        };
    }




    private static ColorResult ToResult(this Color color)
    {
        return new ColorResult
        {
            Name = color.Name,
            Hex = color.Hex
        };
    }

    private static SizeResult ToResult(this Size size)
    {
        return new SizeResult
        {
            Code = size.Code.ToString(),
            Name = size.Name
        };
    }

    private static StatusResult ToResult(this Status s)
    {
        return new StatusResult
        {
            IsNew = s.IsNew,
            IsFeatured = s.IsFeatured,
            IsTrending = s.IsTrending,
            IsBestSeller = s.IsBestSeller
        };
    }

    private static AttributeResults ToResult(this Attribute attribute)
    {
        return new AttributeResults()
        {
            Name = attribute.Name,
            Description = attribute.Description.Value
        };
    }
    private static RatingResult ToResult(this Rating rating)
    {
        return new RatingResult()
        {
            Value = rating.Value,
            Description = rating.Description
        };
    }
}
