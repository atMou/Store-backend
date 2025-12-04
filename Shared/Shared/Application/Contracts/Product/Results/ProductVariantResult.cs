namespace Shared.Application.Contracts.Product.Results;
public record ProductVariantResult
{
    public Guid Id { get; init; }
    public string Slug { get; init; } = null!;
    public decimal Price { get; init; }
    public decimal? NewPrice { get; init; }
    public string Sku { get; init; } = null!;
    public string Size { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string ColorHex { get; set; } = null!;
    public int Stock { get; set; }

}