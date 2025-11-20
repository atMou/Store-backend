using Product.Application.Features.CreateProduct;

namespace Product.Application.Contracts.Commands;

public static class Extensions
{
    public static CreateProductDto ToDto(this CreateProductCommand command)
    {
        return new CreateProductDto()
        {
            Slug = command.Slug,
            //ProductImages = images.ToArray(),
            Stock = command.Stock,
            LowStockThreshold = command.LowStockThreshold,
            MidStockThreshold = command.MidStockThreshold,
            HighStockThreshold = command.HighStockThreshold,
            Price = command.Price,
            NewPrice = command.NewPrice,
            Brand = command.Brand,
            Size = command.Size,
            Color = command.Color,
            Category = command.Category,
            Description = command.Description
        };
    }

}
