namespace Product.Application.Features.GetProducts;

public record GetProductsQuery : IQuery<Fin<PaginatedResult<ProductResult>>>, IPagination, IInclude
{
    public string? Category { get; init; }
    public string? Brand { get; init; }
    public string? Color { get; init; }
    public string? Size { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public string? Search { get; init; }
    public string? OrderBy { get; init; }
    public string? SortDir { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public string? Include { get; init; }
    public bool? IsFeatured { get; init; }
    public bool? IsTrending { get; init; }
    public bool? IsBestSeller { get; init; }
    public bool? IsNew { get; init; }
}

public record GetProductsQueryResult(PaginatedResult<ProductResult> PaginatedResult);

internal class GetProductsQueryHandler(ProductDBContext dbContext)
    : IQueryHandler<GetProductsQuery, Fin<PaginatedResult<ProductResult>>>
{
    public Task<Fin<PaginatedResult<ProductResult>>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var db = GetEntitiesWithPagination<ProductDBContext,
            Domain.Models.Product,
            ProductResult, GetProductsQuery>(
            query,
            product => product.ToResult(),
            null,
            options => QueryEvaluator.Evaluate(options, query)
        );
        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }



}

