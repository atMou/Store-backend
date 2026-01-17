using Product.Application.Features.CreateProduct;

namespace Product.Presentation.Requests;

public static class Extensions
{


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
            Variants = request.ColorVariants.Select(v => v.ToCommand()),
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
    private static CreateColorVariantCommand ToCommand(this CreateColorVariantRequest request) => new CreateColorVariantCommand
    {
        Images = request.Images,
        IsMain = request.IsMain,
        Color = request.Color,
    };

}


