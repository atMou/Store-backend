using Product.Domain.Models;

using Shared.Infrastructure.Images;

namespace Product.Application.Features.UpsertReview;

public record UpsertReviewCommand(ProductId ProductId, string Comment, double Rating)
    : ICommand<Fin<Unit>>
{
}

internal class DeleteImagesCommandHandler(
    ProductDBContext dbContext,
    IUserContext userContext)
    : ICommandHandler<UpsertReviewCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(UpsertReviewCommand command,
        CancellationToken cancellationToken)
    {
        var db =
            from user in userContext.GetCurrentUser<IO>().As()
            from a in GetUpdateEntity<ProductDBContext, Domain.Models.Product>(
                p => p.Id == command.ProductId,
                NotFoundError.New($"Product with ID {command.ProductId} not found"),
                opt =>
                {
                    opt.AsSplitQuery = true;
                    opt = opt.AddInclude(p => p.Reviews);
                    return opt;
                },
                p => Optional(p.Reviews.FirstOrDefault(r =>
                        r.UserId.Value == user.Id && r.ProductId.Value == command.ProductId.Value)).ToFin()
                    .BiBind(
                        review => review.Update(command.Comment, command.Rating).Map(_ => p),
                        _ =>
                            Review.Create(UserId.From(user.Id), command.ProductId, command.Comment, command.Rating, user.Name, user.AvatarUrl)
                                .Map(r =>
                                {
                                    p.AddReview(r);
                                    return p;
                                })
                    )
            )
            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}