namespace Shared.Application.Contracts.Product.Results;
public record ProductResult
{
    public Guid Id { get; init; }
    public string Slug { get; init; }
    public string Brand { get; init; }
    public decimal Price { get; init; }
    public string Category { get; init; }
    public string SubCategory { get; init; }


    public string Description { get; init; }
    public decimal? NewPrice { get; init; }
    public decimal? Discount { get; init; }

    public int TotalReviews { get; init; }
    public int TotalSales { get; init; }
    public ProductTypeResult ProductType { get; init; }
    public RatingResult Rating { get; init; }
    public StatusResult Status { get; init; }

    public IEnumerable<ColorResult> Colors { get; init; }
    public IEnumerable<SizeResult> Sizes { get; init; }
    public IEnumerable<VariantsResult> Variants { get; init; }
    public IEnumerable<ImageResult> Images { get; init; }
    public IEnumerable<ProductResult> Alternatives { get; init; }
    public IEnumerable<ReviewResult> Reviews { get; init; }
    public IEnumerable<AttributeResults> DetailsAttributes { get; init; }
    public IEnumerable<AttributeResults> SizeFitAttributes { get; init; }
    public IEnumerable<MaterialDetailResult> MaterialDetails { get; init; }


}

public record MaterialDetailResult
{
    public Guid Id { get; init; }
    public string Detail { get; init; }
    public decimal Percentage { get; init; }
    public string Material { get; init; }
}

public record ColorResult
{
    public string Name { get; init; }
    public string Hex { get; init; }


}

public record SizeResult
{
    public string Code { get; init; }
    public string Name { get; init; }

}

public record StatusResult
{
    public bool IsNew { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsTrending { get; init; }
    public bool IsBestSeller { get; init; }
}

public record VariantsResult
{
    public Guid Id { get; set; }
    public string Sku { get; init; }
    public ColorResult Color { get; init; }

    public SizeResult Size { get; init; }
    public IEnumerable<ImageResult> Images { get; init; }


}
public record StockResult
{
    public int Stock { get; init; }
    public int Low { get; init; }
    public int Mid { get; init; }
    public int High { get; init; }
}
public record ImageResult
{
    public Guid Id { get; set; }
    public string Url { get; init; }
    public string AltText { get; init; }
    public bool IsMain { get; init; }
}

public record AttributeResults
{
    public string Name { get; init; }
    public string Description { get; init; }
}

public record RatingResult
{
    public double Value { get; init; }
    public string Description { get; init; }
}

public record CategoryResult
{
    public string Main { get; init; }
    public string Sub { get; init; }
    public IEnumerable<ProductTypeCategoryResult> ProductTypes { get; init; } = [];


}

public record ProductTypeResult
{
    public string Type { get; init; }
    public string SubType { get; init; }

}
public record ProductTypeCategoryResult
{
    public string Type { get; init; }
    public IEnumerable<string> SubTypes { get; init; }

}


public record ReviewResult
{
    public Guid Id { get; init; }
    public string Comment { get; init; } = null!;
    public double Rating { get; init; }
    public string RatingDescription { get; init; } = null!;
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid ProductId { get; set; }
}