namespace Product.Domain.Contracts;
public record CreateProductDto
{
    public string Slug { get; init; } = null!;
    public string Brand { get; init; } = null!;
    public string Size { get; init; } = null!;
    public string Color { get; init; } = null!;
    public string Category { get; init; } = null!;
    public string Description { get; init; } = null!;
    public ImageDto[] Images { get; init; } = [];
    public decimal Price { get; init; }
    public decimal? NewPrice { get; init; }
    public int Stock { get; init; }
    public int LowStockThreshold { get; init; }
    public int MidStockThreshold { get; init; }
    public int HighStockThreshold { get; init; }
}
