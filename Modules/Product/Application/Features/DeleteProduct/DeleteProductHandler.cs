namespace Product.Application.Features.DeleteProduct;

public record DeleteProductCommand(ProductId ProductId) : ICommand<Fin<DeleteProductCommandResult>>;
public record DeleteProductCommandResult(bool IsDeleted);

internal class DeleteProductHandlerCommandHandler(ProductDBContext dbContext, IProductRepository productRepository, ISender sender)
    : ICommandHandler<DeleteProductCommand, Fin<DeleteProductCommandResult>>
{
    public async Task<Fin<DeleteProductCommandResult>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var db =
            from _ in CanDeleteProduct(request.ProductId, sender)
            from isDeleted in Db<ProductDBContext>.liftIO((ctx) => productRepository.DeleteProduct(request.ProductId, ctx))
            select new DeleteProductCommandResult(isDeleted);

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private static IO<Unit> CanDeleteProduct(ProductId productId, ISender sender) =>
        from x in IO.liftAsync(async e => await sender.Send(new IsProductInAnyCartsQuery(productId), e.Token))
        select unit;

}