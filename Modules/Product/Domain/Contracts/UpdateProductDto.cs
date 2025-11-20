namespace Product.Domain.Contracts;

public record UpdateProductDto
{
    public ProductId ProductId { get; init; } = null!;
    public string Slug { get; init; } = null!;
    public string Brand { get; init; } = null!;
    public string Color { get; init; } = null!;
    public string Size { get; init; } = null!;
    public bool IsFeatured { get; init; }
    public bool IsNew { get; init; }
    public bool IsBestSeller { get; init; }
    public bool IsTrending { get; init; }
    public decimal Price { get; init; }
    public string Category { get; init; } = null!;
    public string Description { get; init; } = null!;
    public int Stock { get; init; }
    public int LowStockThreshold { get; init; }
    public ImageDto[] ImageDtos { get; init; } = [];
    public ProductId[] VariantsIds { get; init; } = [];
}
