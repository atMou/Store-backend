using Product.Application.Contracts;
using Product.Domain.Models;

using Shared.Application.Contracts.Product.Dtos;
using Shared.Application.Features.Product.Events;
using Shared.Infrastructure.Images;

namespace Product.Application.Features.CreateProduct;

public record CreateProductCommand : ICommand<Fin<Guid>>
{
    public string Slug { get; init; } = null!;
    public IFormFile[] Images { get; init; } = [];
    public bool[] IsMain { get; init; } = [];
    public decimal Price { get; init; }
    public decimal? NewPrice { get; init; }
    public string Brand { get; init; } = null!;
    public string Category { get; init; } = null!;
    public string SubCategory { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Type { get; init; } = null!;
    public string SubType { get; init; } = null!;
    public IEnumerable<CreateVariantCommand> Variants { get; init; } = null!;
    public List<CreateAttributeCommand> DetailsAttributes { get; init; } = null!;
    public List<CreateAttributeCommand> SizeFitAttributes { get; init; } = null!;
    public List<CreateMaterialDetailCommand> MaterialDetails { get; init; } = null!;
}

public record CreateMaterialDetailCommand
{
    public string Material { get; init; } = null!;
    public decimal Percentage { get; init; }
    public string Detail { get; init; } = null!;
}

public record CreateVariantCommand
{
    public IFormFile[] Images { get; init; } = [];
    public bool[] IsMain { get; init; } = [];
    public string Color { get; init; } = null!;
    public IEnumerable<CreateSizeVariantCommand> SizeVariants { get; init; } = null!;

}

public record CreateSizeVariantCommand
{
    public string Size { get; init; } = null!;
    public int Stock { get; init; }
    public int StockLow { get; init; }
    public int StockMid { get; init; }
    public int StockHigh { get; init; }
}

public record CreateAttributeCommand
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
}

internal class CreateProductHandlerCommandHandler(
    ProductDBContext dbContext,
    IPublishEndpoint endpoint,
    IImageService imageService)
    : ICommandHandler<CreateProductCommand, Fin<Guid>>
{
    public async Task<Fin<Guid>> Handle(CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        IEnumerable<VariantDto> variants = [];
        var db =
            from p in AddEntity<ProductDBContext, Domain.Models.Product, CreateProductDto>(
                command.ToDto(),
                Domain.Models.Product.Create,
                product =>
                    UploadProductImages(product, command),
                product => (
                    AddVariant(product, command.Variants, cancellationToken)
                        .Map(vs =>
                        {
                            variants = vs.SelectMany(v => v.VariantDtos).ToList();
                            return product.AddVariants(vs.Select(v => v.Variant));
                        })
                ))
            select p;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken)).RaiseOnSuccess(async p =>
        {
            await endpoint.Publish(new ProductCreatedIntegrationEvent()
            {
                ProductId = p.Id.Value,
                Slug = p.Slug.Value,
                Brand = p.Brand.Name,
                Variants = variants
            }, cancellationToken);
            return p.Id.Value;
        });
    }

    private IO<Domain.Models.Product> UploadProductImages(Domain.Models.Product product, CreateProductCommand command)
    {
        return from ims in imageService.UploadProductImages(command.Images, command.IsMain,
                product.Slug.Value,
                product.Category.ToString(), product.Brand.Name)
               select product.AddImages([.. ims]);
    }

    private IO<Iterable<(Variant Variant, IEnumerable<VariantDto> VariantDtos)>> AddVariant(
        Domain.Models.Product product,
        IEnumerable<CreateVariantCommand> variants,
        CancellationToken token)
    {
        return variants.AsIterable().Traverse(command =>
        {
            return from x in imageService.UploadProductImages(command.Images, command.IsMain, product.Slug.Value,
                    product.Category.ToString(), product.Brand.Name, command.Color)
                   from v in Variant.Create(command.ToDto(), product.Brand.Name, product.Category.ToString())
                       .Map(variant => variant.AddImages([.. x]))
                   select (v, command.SizeVariants.Select(variant => new VariantDto()
                   {
                       Color = v.Color.Name,
                       Size = variant.Size,
                       Sku = v.Sku.Value,
                       Stock = variant.Stock,
                       Low = variant.StockLow,
                       Mid = variant.StockMid,
                       High = variant.StockHigh

                   }));
        }).As();
    }
}