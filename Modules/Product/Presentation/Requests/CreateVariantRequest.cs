using Product.Application.Features.CreateProduct;

namespace Product.Presentation.Requests;

public record CreateVariantRequest
{

    public IFormFile[] Images { get; init; } = [];
    public bool[] IsMain { get; init; } = [];
    public string Color { get; init; } = null!;
    public string Size { get; init; } = null!;
    public int Quantity { get; init; }
    public int StockLow { get; init; }
    public int StockMid { get; init; }
    public int StockHigh { get; init; }

    public IEnumerable<CreateAttributeRequest> Attributes { get; init; } = [];

    public CreateVariantCommand ToCommand() => new CreateVariantCommand
    {
        Images = Images,
        IsMain = IsMain,
        Color = Color,
        Size = Size,
        Quantity = Quantity,
        StockLow = StockLow,
        StockMid = StockMid,
        StockHigh = StockHigh,
        Attributes = Attributes.Select(a => a.ToCommand())
    };

}