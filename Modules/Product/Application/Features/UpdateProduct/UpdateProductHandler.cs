using Shared.Infrastructure.Images;

namespace Product.Application.Features.UpdateProduct;

public record UpdateProductCommand(UpdateProductDto UpdateProductDto)
    : ICommand<Fin<Unit>>
{
}

internal class UpdateProductCommandHandler(
    ProductDBContext dbContext,
    IImageService imageService)
    : ICommandHandler<UpdateProductCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var dto = command.UpdateProductDto;
        var db =
            from ps in GetEntities<ProductDBContext, Domain.Models.Product>(p => dto.AlternativesIds.Contains(p.Id))

            from x in GetUpdateEntity<ProductDBContext, Domain.Models.Product>(
                p => p.Id == dto.ProductId,
                NotFoundError.New($"Product with id '{dto.ProductId}' was not found."),
                opt =>
                {
                    opt.AsSplitQuery = true;
                    opt.AddInclude(p => p.Alternatives, p => p.Variants);
                    return opt;
                },
                p => p.Update(dto, ps),
                p => Seq(UploadProductImages(p, dto), UploadVariantImages(p, dto.Variants))
            )
            select unit;


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private IO<Domain.Models.Product> UploadProductImages(Domain.Models.Product product, UpdateProductDto dto)
    {
        return imageService.UploadProductImages(dto.Images, dto.IsMain, product.Slug.Value, product.Category.Name,
                product.Brand.Name)
            .Map(results => product.AddImages(
                [.. results]));
    }

    private IO<Domain.Models.Product> UploadVariantImages(Domain.Models.Product product,
        IEnumerable<UpdateVariantDto> dtos)
    {
        return dtos.AsIterable().Traverse(dto =>
        {
            return imageService.UploadProductImages(dto.Images, dto.IsMain, product.Slug.Value,
                product.Category.Name, product.Brand.Name, dto.Color, dto.Size).Map(results =>
                product.AddImages(dto.VariantId,
                    [.. results]));
        }).Map(_ => product).As();
    }
}