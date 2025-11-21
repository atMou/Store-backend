using Product.Application.Features.CreateProduct;

namespace Product.Presentation.Requests;

public record CreateProductRequest
{
    public string Slug { get; init; } = null!;
    public IFormFile[] Images { get; init; } = [];
    public bool[] IsMain { get; init; } = [];
    public string Brand { get; init; } = null!;
    public string Size { get; init; } = null!;
    public string Color { get; init; } = null!;
    public string Category { get; init; } = null!;
    public int Stock { get; init; }
    public int LowStockThreshold { get; init; }
    public int MidStockThreshold { get; init; }
    public int HighStockThreshold { get; init; }
    public decimal Price { get; init; }
    public decimal? NewPrice { get; init; }
    public string Description { get; init; } = null!;


    public CreateProductCommand ToCommand()
    {
        return new CreateProductCommand
        {
            Slug = Slug,
            Images = Images,
            IsMain = IsMain,
            Stock = Stock,
            Price = Price,
            NewPrice = NewPrice,
            Brand = Brand,
            Size = Size,
            Color = Color,
            Category = Category,
            Description = Description,
            LowStockThreshold = LowStockThreshold,
            MidStockThreshold = MidStockThreshold,
            HighStockThreshold = HighStockThreshold,
        };
    }
}


