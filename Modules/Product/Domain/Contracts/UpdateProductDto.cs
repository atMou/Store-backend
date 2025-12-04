namespace Product.Domain.Contracts;

public record UpdateProductDto
{
    public ProductId ProductId { get; init; } = null!;
    public bool[] IsMain { get; set; }
    public IFormFile[] Images { get; init; }
    public string Slug { get; init; }
    public string Brand { get; init; }
    public string Category { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsNew { get; init; }
    public bool IsBestSeller { get; init; }
    public bool IsTrending { get; init; }
    public decimal Price { get; init; }
    public decimal NewPrice { get; init; }
    public string Description { get; init; }
    public IEnumerable<UpdateProductImageDto> ImageDtos { get; init; } = [];
    public IEnumerable<UpdateVariantDto> Variants { get; set; }

    public IEnumerable<ProductId> AlternativesIds { get; init; }
}
