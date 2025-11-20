

namespace Product.Application.Features.UpdateProduct;

internal abstract record UpdateProductCommand(
    UpdateProductDto UpdateProductDto
) : ICommand<Fin<UpdateProductCommandResult>>;
internal record UpdateProductCommandResult(bool IsUpdated);

internal class UpdateProductCommandHandler(ProductDBContext dbContext, IProductRepository productRepository)
    : ICommandHandler<UpdateProductCommand, Fin<UpdateProductCommandResult>>
{
    public async Task<Fin<UpdateProductCommandResult>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var db = from updatedProduct in Db<ProductDBContext>.liftIO(ctx => productRepository.UpdateProduct(command.UpdateProductDto, ctx,
                opt =>
                {
                    opt.AsSplitQuery = true;
                    opt.AddInclude(p => p.ProductImages, p => p.Reviews, p => p.Variants);
                }))

                 select new UpdateProductCommandResult(true);
        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}