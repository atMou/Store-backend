namespace Product.Application.Features.DeleteProduct;

public record DeleteProductCommand(ProductId ProductId) : ICommand<Fin<Unit>>;

internal class DeleteProductHandlerCommandHandler(ProductDBContext dbContext, IProductRepository productRepository, ISender sender)
    : ICommandHandler<DeleteProductCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var db =
            from _ in CanDeleteProduct(request.ProductId, sender)
            from isDeleted in Db<ProductDBContext>.liftIO((ctx) =>
                productRepository.DeleteProduct(request.ProductId, ctx))
            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private static IO<Unit> CanDeleteProduct(ProductId productId, ISender sender) =>
        from x in IO.liftAsync(async e => await sender.Send(new IsProductInAnyCartsQuery(productId), e.Token))
        select unit;

}