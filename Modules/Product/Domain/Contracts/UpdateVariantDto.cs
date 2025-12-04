namespace Product.Domain.Contracts;

public record UpdateVariantDto
{
    public VariantId VariantId { get; init; } = null!;
    public bool[] IsMain { get; set; }
    public IFormFile[] Images { get; init; }
    public string Color { get; init; }
    public string Size { get; init; }
    public int StockLow { get; init; }
    public int StockMid { get; init; }
    public int StockHigh { get; init; }
    public int Stock { get; init; }
    public IEnumerable<UpdateVariantImageDto> ImageDtos { get; init; } = [];
    public IEnumerable<UpdateAttributeDto> Attributes { get; set; }

}
