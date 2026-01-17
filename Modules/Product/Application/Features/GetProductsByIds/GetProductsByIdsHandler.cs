using Microsoft.Extensions.Caching.Distributed;

using Shared.Application.Contracts;

namespace Product.Application.Features.GetProductsByIds;

internal class GetProductsQueryHandler(ProductDBContext dbContext, IDistributedCache cache)
    : IQueryHandler<GetProductsByIdsQuery, Fin<PaginatedResult<ProductResult>>>
{
    public async Task<Fin<PaginatedResult<ProductResult>>> Handle(GetProductsByIdsQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = query.GetCacheKey();
        var db = GetEntitiesWithPagination<ProductDBContext,
            Domain.Models.Product,
            ProductResult, GetProductsByIdsQuery>(
            p => query.ProductIds.Contains(p.Id),
            options => QueryEvaluator.Evaluate(options, query),
            query,
            product => product.ToResult()
        );
        return await db.WithPaginatedCache<Domain.Models.Product, ProductResult, ProductDBContext, IDistributedCache>(
            cacheKey,
            cache,
            dbContext,
            EnvIO.New(null, cancellationToken));

    }


}

