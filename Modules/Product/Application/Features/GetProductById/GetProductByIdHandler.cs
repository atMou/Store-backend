using Product.Domain.Contracts;
using Product.Persistence.Data;

using Shared.Application.Abstractions;
using Shared.Application.Contracts.Product.Queries;
using Shared.Domain.Errors;

namespace Product.Application.Features.GetProductById;

internal class GetProductByIdQueryHandler(ProductDBContext dbContext)
    : IQueryHandler<GetProductByIdQuery, Fin<GetProductByIdResult>>
{
    public Task<Fin<GetProductByIdResult>> Handle(GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        return (from p in Db<ProductDBContext>.liftIO(async (ctx, e) =>
                    await ctx.Products.FindAsync([request.ProductId.Value], e.Token))
                from a in when(p is null,
                    Db<ProductDBContext>.fail<Unit>(
                        NotFoundError.New($"Product with id: '{request.ProductId.Value} was not found'")))
                select new GetProductByIdResult(p.ToDto())
            ).RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}