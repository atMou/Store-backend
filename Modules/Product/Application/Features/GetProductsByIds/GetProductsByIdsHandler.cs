using Shared.Application.Contracts;

namespace Product.Application.Features.GetProductsByIds;
public record GetProductsQueryResult(PaginatedResult<ProductResult> PaginatedResult);

internal class GetProductsQueryHandler(ProductDBContext dbContext)
    : IQueryHandler<GetProductsByIdsQuery, Fin<PaginatedResult<ProductResult>>>
{
    public async Task<Fin<PaginatedResult<ProductResult>>> Handle(GetProductsByIdsQuery query, CancellationToken cancellationToken)
    {
        var db = GetEntitiesWithPagination<ProductDBContext,
            Domain.Models.Product,
            ProductResult, GetProductsByIdsQuery>(
            p => query.ProductIds.Contains(p.Id),
            options => QueryEvaluator.Evaluate(options, query),
            query,
            product => product.ToResult()
        );
        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

}

