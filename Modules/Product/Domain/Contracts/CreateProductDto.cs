namespace Product.Domain.Contracts;
public record CreateProductDto
{
    public string Slug { get; init; } = null!;
    public string Brand { get; init; } = null!;
    public string Category { get; init; } = null!;
    public string SubCategory { get; init; } = null!;

    public string Type { get; init; } = null!;
    public string SubType { get; init; } = null!;

    public string Description { get; init; } = null!;
    public decimal Price { get; init; }
    public decimal? NewPrice { get; init; }
    public IEnumerable<CreateAttributeDto> DetailsAttributes { get; init; }
    public IEnumerable<CreateAttributeDto> SizeFitAttributes { get; init; }
    public IEnumerable<CreateMaterialDetailDto> MaterialDetails { get; init; }

}

public record CreateAttributeDto
{
    public string Name { get; init; }
    public string Description { get; init; }
}
public record CreateMaterialDetailDto
{
    public string Material { get; init; }
    public decimal Percentage { get; init; }
    public string Detail { get; init; }

}

public record CreateSizeVariantDto
{
    public string Size { get; init; } = null!;
    public int Stock { get; init; }
    public int StockLow { get; init; }
    public int StockMid { get; init; }
    public int StockHigh { get; init; }

}

public record CreateVariantDto
{
    public string Color { get; init; }
    public IEnumerable<CreateSizeVariantDto> SizeVariants { get; init; }


}
