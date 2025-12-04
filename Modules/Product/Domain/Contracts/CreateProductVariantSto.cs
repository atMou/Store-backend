namespace Product.Domain.Contracts;

public record CreateVariantDto
{
    public string Color { get; init; }
    public string Size { get; init; }
    public int Stock { get; init; }
    public int StockLow { get; init; }
    public int StockMid { get; init; }
    public int StockHigh { get; init; }
    public IEnumerable<CreateAttributeDto> Attributes { get; init; }
}
