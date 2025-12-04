namespace Shared.Application.Contracts.Product.Results;

public record ProductVariantsResult
{
    public Guid Id { get; set; }
    public string Sku { get; init; }
    public string Color { get; set; }
    public string ColorHex { get; set; }
    public string Size { get; set; }
    public string StockLevel { get; set; }

    public IEnumerable<ImageResult> Images { get; set; }
}