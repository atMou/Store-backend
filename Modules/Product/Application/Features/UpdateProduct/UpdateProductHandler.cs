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
                    opt = opt.AddInclude(p => p.Alternatives, p => p.Variants);
                    return opt;
                },
                p => p.Update(dto, ps),
                p =>
                    from p1 in UploadProductImages(p, dto)
                    from p2 in UploadVariantImages(p1, dto.Variants)
                    from _ in DeleteImagesMarkedAsDeleted(p2, dto)
                    select p2)
            select unit;


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private IO<Domain.Models.Product> UploadProductImages(Domain.Models.Product product, UpdateProductDto dto)
    {
        return imageService.UploadProductImages(dto.Images, dto.IsMain, product.Slug.Value, product.Category.ToString(),
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
                product.Category.ToString(), product.Brand.Name, dto.Color)
                .Map(results =>
                product.AddImages(dto.VariantId, [.. results]));
        }).Map(_ => product).As();
    }


    private IO<Unit> DeleteImagesMarkedAsDeleted(
        Domain.Models.Product product,
        UpdateProductDto dto)
    {
        var deletedProductImageIds = dto.ImageDtos
            .Where(img => img.IsDeleted)
            .Select(img => img.ImageId)
            .ToHashSet();

        var deletedVariantImageIds = dto.Variants
            .SelectMany(v => v.ImageDtos)
            .Where(img => img.IsDeleted)
            .Select(img => img.ImageId)
            .ToHashSet();

        var productPublicIds = product.Images
            .Where(image => deletedProductImageIds.Contains(image.Id))
            .Select(image => image.ImageUrl.PublicId);

        var variantPublicIds = product.Variants
            .SelectMany(variant => variant.Images)
            .Where(image => deletedVariantImageIds.Contains(image.Id))
            .Select(image => image.ImageUrl.PublicId);

        var allPublicIds = productPublicIds.Concat(variantPublicIds).ToList();

        return allPublicIds.Any()
            ? imageService.DeleteImagesAsync(allPublicIds)
            : IO<Unit>.Pure(unit);
    }
}