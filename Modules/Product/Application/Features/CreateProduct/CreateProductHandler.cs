using Product.Application.Contracts.Commands;

using Shared.Infrastructure.Images;

namespace Product.Application.Features.CreateProduct;


public record CreateProductCommand : ICommand<Fin<CreateProductResult>>
{
    public string Slug { get; init; } = null!;
    public IFormFile[] Images { get; init; } = [];
    public bool[] IsMain { get; init; } = [];
    public int Stock { get; init; }
    public int LowStockThreshold { get; init; }
    public int MidStockThreshold { get; init; }
    public int HighStockThreshold { get; init; }
    public decimal Price { get; init; }
    public decimal? NewPrice { get; init; }
    public string Brand { get; init; } = null!;
    public string Size { get; init; } = null!;
    public string Color { get; init; } = null!;
    public string Category { get; init; } = null!;
    public string Description { get; init; } = null!;
}


public record CreateProductResult(Guid Id);

internal class CreateProductHandlerCommandHandler(
    ProductDBContext dbContext,
    IImageService imageService)
    : ICommandHandler<CreateProductCommand, Fin<CreateProductResult>>
{
    public async Task<Fin<CreateProductResult>> Handle(CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from product in Domain.Models.Product.Create(command.ToDto())
            from imageDtos in imageService.UploadProductImages(command.Images, command.IsMain, product.Slug.Value, product.Category.Name, product.Brand.Name, product.Color.Name)
            from p in product.AddImages(imageDtos.ToArray())
            from _ in Db<ProductDBContext>.lift(ctx => ctx.Add(p))
            select new CreateProductResult(p.Id.Value);
        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

}



