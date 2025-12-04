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
            Description = command.Description
        };
    }
    public static CreateVariantDto ToDto(this CreateVariantCommand command)
    {
        return new CreateVariantDto()
        {
            Color = command.Color,
            Size = command.Size,
            Stock = command.Quantity,
            StockLow = command.StockLow,
            StockMid = command.StockMid,
            StockHigh = command.StockHigh,
            Attributes = command.Attributes.Select(a => a.ToDto())
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
