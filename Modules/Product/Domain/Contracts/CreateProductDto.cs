namespace Product.Domain.Contracts;
public record CreateProductDto
{
    public string Slug { get; init; } = null!;
    public string Brand { get; init; } = null!;
    public string Category { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal Price { get; init; }
    public decimal? NewPrice { get; init; }
    public IEnumerable<CreateVariantDto> Variants { get; init; }

}
