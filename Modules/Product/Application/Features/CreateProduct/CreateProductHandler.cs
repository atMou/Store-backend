using Product.Application.Contracts;
using Product.Domain.Models;

using Shared.Infrastructure.Images;

namespace Product.Application.Features.CreateProduct;

public record CreateProductCommand : ICommand<Fin<CreateProductResult>>
{
    public string Slug { get; init; } = null!;
    public IFormFile[] Images { get; init; } = [];
    public bool[] IsMain { get; init; } = [];
    public decimal Price { get; init; }
    public decimal? NewPrice { get; init; }
    public string Brand { get; init; } = null!;
    public string Category { get; init; } = null!;
    public string Description { get; init; } = null!;
    public IEnumerable<CreateVariantCommand> Variants { get; init; } = [];
}

public record CreateVariantCommand
{
    public IFormFile[] Images { get; init; } = [];
    public bool[] IsMain { get; init; } = [];
    public string Color { get; init; } = null!;
    public string Size { get; init; } = null!;
    public int Quantity { get; init; }
    public int StockLow { get; init; }
    public int StockMid { get; init; }
    public int StockHigh { get; init; }

    public IEnumerable<CreateAttributeCommand> Attributes { get; init; } = [];
}

public record CreateAttributeCommand
{
    public string Name { get; init; } = null!;
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
            from p in AddEntity<ProductDBContext, Domain.Models.Product, CreateProductDto>(
                command.ToDto(),
                dto => Domain.Models.Product.Create(dto),
                product => Seq(
                    UploadProductImages(product, command),
                    AddVariants(product, command.Variants, cancellationToken)
                ))
            select new CreateProductResult(p.Id.Value);


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


    private IO<Domain.Models.Product> UploadProductImages(Domain.Models.Product product, CreateProductCommand command)
    {
        return from ims in imageService.UploadProductImages(command.Images, command.IsMain, product.Slug.Value,
                product.Category.Name, product.Brand.Name)
               let p = product.AddImages([.. ims])
               select p;
    }

    private IO<Domain.Models.Product> AddVariants(Domain.Models.Product product, IEnumerable<CreateVariantCommand> dtos,
        CancellationToken token)
        => from res in dtos.AsIterable().Traverse(vc =>
                from ims in imageService.UploadProductImages(
                    vc.Images, vc.IsMain, product.Slug.Value, product.Category.Name, product.Brand.Name, vc.Color,
                    vc.Size)
                from variant in Variant.Create(vc.ToDto(), product.Category.Name, product.Brand.Name)
                    .Map(v => v.AddImages([.. ims]))
                select variant
            ).Map(vs => product.AddVariants(vs)).As()
           select res;
}