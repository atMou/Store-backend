

using Shared.Infrastructure.Images;

namespace Product.Application.Features.UpdateProduct;

public record UpdateProductCommand(UpdateProductDto UpdateProductDto)
    : ICommand<Fin<Unit>>
{

}

internal class UpdateProductCommandHandler(
    ProductDBContext dbContext,
    IProductRepository productRepository,
    IImageService imageService)
    : ICommandHandler<UpdateProductCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var dto = command.UpdateProductDto;

        var loadProduct =
            from p in Db<ProductDBContext>.liftIO(async (ctx, e) => await
                ctx.Products.WithQueryOptions(opt =>
                    {
                        opt.AsSplitQuery = true;
                        opt.AddInclude(p => p.ProductImages, p => p.Reviews, p => p.Variants);
                    })
                    .FirstOrDefaultAsync(product => product.Id == dto.ProductId, e.Token))
            from _ in when(p is null, IO.fail<Unit>(NotFoundError.New($"Product with id '{dto.ProductId}' was not found.")))
            select p;

        var loadVariants = from vs in Db<ProductDBContext>.liftIO(async (ctx, e) =>
                await ctx.Products.Where(p => dto.VariantsIds.Contains(p.Id)).ToListAsync(e.Token))
                           select vs;


        var db =
            from t in (loadVariants, loadProduct).Apply((vs, p) => (vs, p))
            from ims in imageService.UploadProductImages(dto.Images, dto.IsMain, t.p.Slug.Value, t.p.Category.Name,
                t.p.Brand.Name, t.p.Color.Name)
            from updatedProduct in t.p.Update(dto, t.vs, ims.ToList()).Match(
                prod => IO.lift(() => prod),
                IO.fail<Domain.Models.Product>)

            from _ in Db<ProductDBContext>.lift(ctx =>
            {
                ctx.Products.Entry(t.p).CurrentValues.SetValues(updatedProduct);
                return unit;
            })
            select unit;


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}