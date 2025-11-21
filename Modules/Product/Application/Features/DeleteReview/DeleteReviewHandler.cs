namespace Product.Application.Features.DeleteReview;

public record DeleteReviewCommand(ProductId ProductId, ReviewId ReviewId)
    : ICommand<Fin<Unit>>
{
}

internal class DeleteImagesCommandHandler(ProductDBContext dbContext)
    : ICommandHandler<DeleteReviewCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(DeleteReviewCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from product in Db<ProductDBContext>.liftIO((ctx, e) =>
                ctx.Products.Include(p => p.Reviews)
                    .FirstOrDefaultAsync(p => p.Id == command.ProductId, e.Token))
            let er = product?.Reviews.FirstOrDefault(r => r.Id == command.ReviewId)
            from review in when(er is null,
                IO.fail<Unit>(
                    NotFoundError.New(
                        $"Review with Id '{command.ReviewId}' not found for Product '{command.ProductId}'.")))
            let updatedProduct = product.DeleteReview(er)
            from x in Db<ProductDBContext>.lift(ctx =>
            {
                ctx.Products.Entry(product).CurrentValues.SetValues(updatedProduct);
                return unit;
            })
            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}