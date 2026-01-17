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
    public IEnumerable<CreateAttributeDto> DetailsAttributes { get; init; } = null!;
    public IEnumerable<CreateAttributeDto> SizeFitAttributes { get; init; } = null!;
    public IEnumerable<CreateMaterialDetailDto> MaterialDetails { get; init; } = null!;

}

public record CreateAttributeDto
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
}
public record CreateMaterialDetailDto
{
    public string Material { get; init; } = null!;
    public decimal Percentage { get; init; }
    public string Detail { get; init; } = null!;

}

public record CreateColorVariantDto
{
    public string Color { get; init; } = null!;

}
