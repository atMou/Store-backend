using Product.Application.Contracts;
using Product.Domain.Models;

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
    public IEnumerable<CreateColorVariantCommand> Variants { get; init; } = null!;
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

public record CreateColorVariantCommand
{
    public IFormFile[] Images { get; init; } = [];
    public bool[] IsMain { get; init; } = [];
    public string Color { get; init; } = null!;
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
        var createProductDto = command.ToDto();
        var db =
            from p in AddEntity<ProductDBContext, Domain.Models.Product, CreateProductDto>(
                createProductDto,
                Domain.Models.Product.Create,
                product =>
                    UploadProductImages(product, command),
                product => UploadVariantsImages(product, command.Variants)
                    .Map(vs => product.AddColorVariants(vs.ToArray())))
            select p;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken)).RaiseOnSuccess(async p
            =>
        {
            await endpoint.Publish(
                new ProductCreatedIntegrationEvent
                {
                    ProductId = p.Id.Value,
                    Brand = p.Brand.Code.ToString(),
                    Slug = p.Slug.Value,
                    ImageUrl = p.Images.FirstOrDefault(image => image.IsMain)?.ImageUrl.Value ??
                               p.Images.First().ImageUrl.Value,
                    ColorVariants = p.ColorVariants.Select(cv => new Shared.Application.Features.Product.Events.CreateColorVariantDto
                    {
                        ColorVariantId = cv.Id.Value,
                        Color = cv.Color.Name
                    })
                },
                cancellationToken);
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

    private IO<IEnumerable<ColorVariant>> UploadVariantsImages(
        Domain.Models.Product product,
        IEnumerable<CreateColorVariantCommand> commands)
    {
        return commands.AsIterable().Traverse(command =>
        {
            return from x in imageService.UploadProductImages(command.Images, command.IsMain, product.Slug.Value,
                    product.Category.ToString(), product.Brand.Name, command.Color)
                   from v in ColorVariant.Create(command.ToDto())
                       .Map(variant => variant.AddImages([.. x]))
                   select v;
        }).Map(it => it.AsEnumerable()).As();
    }
}