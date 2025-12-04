namespace Product.Application.Features.GetProductsByIds;
public record GetProductsQueryResult(PaginatedResult<ProductResult> PaginatedResult);

internal class GetProductsQueryHandler(ProductDBContext dbContext)
    : IQueryHandler<GetProductsByIdsQuery, Fin<PaginatedResult<ProductResult>>>
{
    public Task<Fin<PaginatedResult<ProductResult>>> Handle(GetProductsByIdsQuery query, CancellationToken cancellationToken)
    {
        var db = GetEntitiesWithPagination<ProductDBContext,
            Domain.Models.Product,
            ProductResult, GetProductsByIdsQuery>(
            query,
            product => product.ToResult(),
            p => query.ProductIds.Contains(p.Id),
            options => QueryEvaluator.Evaluate(options, query)
        );
        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

}

