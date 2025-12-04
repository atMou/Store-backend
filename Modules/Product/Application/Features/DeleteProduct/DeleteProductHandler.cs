using Product.Persistence;

namespace Product.Application.Features.DeleteProduct;

public record DeleteProductCommand(ProductId ProductId) : ICommand<Fin<Unit>>;

internal class DeleteProductHandlerCommandHandler(ProductDBContext dbContext, ISender sender)
    : ICommandHandler<DeleteProductCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var db =
            from _1 in SoftDeleteEntity<ProductDBContext, Domain.Models.Product>(
                product => product.Id == request.ProductId,
                product => product.MarkAsDeleted()
                , NotFoundError.New($"Product with ID {request.ProductId} not found"))
            from _ in CanDeleteProduct(request.ProductId, sender)
            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private static IO<Unit> CanDeleteProduct(ProductId productId, ISender sender) =>
        from x in IO.liftAsync(async e => await sender.Send(new EnsureProductNotInCartsQuery(productId), e.Token))
        select unit;

}