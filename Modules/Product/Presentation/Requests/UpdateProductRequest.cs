using Product.Application.Features.UpdateProduct;

namespace Product.Presentation.Requests;

public record UpdateProductRequest
{
    public ProductId ProductId { get; init; } = null!;
    public IFormFile[] Images { get; init; }
    public bool[] IsMain { get; set; }
    public string Slug { get; init; }
    public string Brand { get; init; }
    public string Color { get; init; }
    public string Size { get; init; }
    public string Category { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsNew { get; init; }
    public bool IsBestSeller { get; init; }
    public bool IsTrending { get; init; }
    public decimal Price { get; init; }
    public decimal NewPrice { get; init; }
    public string Description { get; init; }
    public int Stock { get; init; }
    public int LowStockThreshold { get; init; }
    public int MidStockThreshold { get; init; }
    public int HighStockThreshold { get; init; }
    public ProductId[] VariantsIds { get; init; }


    public UpdateProductCommand ToCommand()
    {
        return new UpdateProductCommand(
            new UpdateProductDto()
            {
                ProductId = ProductId,
                Images = Images,
                IsMain = IsMain,
                Slug = Slug,
                Brand = Brand,
                Color = Color,
                Size = Size,
                Category = Category,
                IsFeatured = IsFeatured,
                IsNew = IsNew,
                IsBestSeller = IsBestSeller,
                IsTrending = IsTrending,
                Price = Price,
                NewPrice = NewPrice,
                Description = Description,
                Stock = Stock,
                LowStockThreshold = LowStockThreshold,
                MidStockThreshold = MidStockThreshold,
                HighStockThreshold = HighStockThreshold,
                VariantsIds = VariantsIds

            }
        );

    }
}