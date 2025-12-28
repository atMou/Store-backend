using Product.Application.Features.CreateProduct;

namespace Product.Presentation.Requests;



public static class Extensions
{
    private static CreateVariantCommand ToCommand(this CreateVariantRequest request) => new CreateVariantCommand
    {
        Images = request.Images,
        IsMain = request.IsMain,
        Color = request.Color,
        ,
    };
    public static CreateProductCommand ToCommand(this CreateProductRequest request)
    {
        return new CreateProductCommand
        {
            Slug = request.Slug,
            Images = request.Images,
            IsMain = request.IsMain,
            Brand = request.Brand,
            Category = request.Category,
            SubCategory = request.SubCategory,
            Price = request.Price,
            NewPrice = request.NewPrice,
            Description = request.Description,
            Variants = request.Variants.Select(v => v.ToCommand()),
            DetailsAttributes = request.DetailsAttributes.Select(a => a.ToCommand()).ToList(),
            SizeFitAttributes = request.SizeFitAttributes.Select(a => a.ToCommand()).ToList(),
            MaterialDetails = request.MaterialDetails.Select(md => new CreateMaterialDetailCommand
            {
                Material = md.Material,
                Percentage = md.Percentage,
                Detail = md.Detail
            }).ToList(),
            Type = request.Type,
            SubType = request.SubType


        };
    }

    private static CreateAttributeCommand ToCommand(this CreateAttributeRequest request) => new()
    {
        Name = request.Name,
        Description = request.Description
    };

    private static CreateSizeVariantCommand ToCommand(this CreateSizeVariantRequest request) => new()
    {
        Size = request.Size,
        Stock = request.Stock,
        StockLow = request.StockLow,
        StockMid = request.StockMid,
        StockHigh = request.StockHigh
    };

}


