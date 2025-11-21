using Product.Domain.Models;

using Shared.Infrastructure.Images;

namespace Product.Application.Features.UpsertReview;

public record UpsertReviewCommand(UserId UserId, ProductId ProductId, string Comment, double Rating)
    : ICommand<Fin<Unit>>
{

}

internal class DeleteImagesCommandHandler(
    ProductDBContext dbContext,
    IImageService imageService)
    : ICommandHandler<UpsertReviewCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(UpsertReviewCommand command,
        CancellationToken cancellationToken)
    {

        var db =
            from product in Db<ProductDBContext>.liftIO((ctx, e) =>
                ctx.Products.Include(p => p.Reviews)
                    .FirstOrDefaultAsync(p => p.Id == command.ProductId, e.Token))

            let er = product?.Reviews.FirstOrDefault(r => r.UserId == command.UserId)

            from updatedProduct in iff(er is null,
                Review.Create(command.UserId, command.ProductId, command.Comment, command.Rating)
                    .Map(review => product.AddReview(review)),
                er.Update(command.Comment, command.Rating)
                    .Map(review => product.UpdateReview(er, review))).As()

            from x in Db<ProductDBContext>.lift(ctx =>
                {
                    ctx.Products.Entry(product).CurrentValues.SetValues(updatedProduct);
                    return unit;
                })

            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}

