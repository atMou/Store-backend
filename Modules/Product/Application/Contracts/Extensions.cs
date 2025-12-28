using Product.Application.Features.CreateProduct;

namespace Product.Application.Contracts;

public static class Extensions
{
    public static CreateProductDto ToDto(this CreateProductCommand command)
    {
        return new CreateProductDto()
        {
            Slug = command.Slug,
            Price = command.Price,
            NewPrice = command.NewPrice,
            Brand = command.Brand,
            Category = command.Category,
            SubCategory = command.SubCategory,
            Description = command.Description,
            Type = command.Type,
            SubType = command.SubType,
            MaterialDetails = command.MaterialDetails.Select(m => m.ToDto()),
            DetailsAttributes = command.DetailsAttributes.Select(a => a.ToDto()),
            SizeFitAttributes = command.SizeFitAttributes.Select(a => a.ToDto())

        };
    }

    public static CreateMaterialDetailDto ToDto(this CreateMaterialDetailCommand command)
    {
        return new CreateMaterialDetailDto()
        {
            Material = command.Material,
            Percentage = command.Percentage,
            Detail = command.Detail
        };
    }

    public static CreateVariantDto ToDto(this CreateVariantCommand command)
    {
        return new CreateVariantDto()
        {
            Color = command.Color,
            SizeVariants = command.SizeVariants.Select(sv => new CreateSizeVariantDto
            {
                Size = sv.Size,
                Stock = sv.Stock,
                StockLow = sv.StockLow,
                StockMid = sv.StockMid,
                StockHigh = sv.StockHigh
            }).ToList()
        };
    }

    public static CreateAttributeDto ToDto(this CreateAttributeCommand command)
    {
        return new CreateAttributeDto()
        {
            Name = command.Name,
            Description = command.Description
        };
    }

}
