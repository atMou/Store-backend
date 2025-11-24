using Shared.Presentation;

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
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string[]? Include { get; init; } = [];

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
            options => QueryEvaluator(options, query)
        );
        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


    private QueryOptions<Domain.Models.Product> QueryEvaluator(QueryOptions<Domain.Models.Product> options,
        GetProductsQuery query)
    {

        options = options with
        {
            AsNoTracking = true,
            AsSplitQuery = true,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize

        };

        if (!string.IsNullOrWhiteSpace(query.Brand))
            options = options.AddFilters(p => p.Brand.Name.Contains(query.Brand));


        if (!string.IsNullOrWhiteSpace(query.Category))
            options = options.AddFilters(p => p.Category.Name.Contains(query.Category));

        if (!string.IsNullOrWhiteSpace(query.Color))
            options = options.AddFilters(p => p.Color.Name.Contains(query.Color));

        if (!string.IsNullOrWhiteSpace(query.Size))
            options = options.AddFilters(p => p.Size.Name.Contains(query.Size));

        if (!string.IsNullOrWhiteSpace(query.Search))
            options = options.AddFilters(p => p.Slug.Value.Contains(query.Search));

        if (query.MinPrice.HasValue)
            options = options.AddFilters(p => p.Price.Value >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            options = options.AddFilters(p => p.Price.Value <= query.MaxPrice.Value);

        if (query.Include is { Length: > 0 })
        {
            foreach (string se in query.Include)
            {
                if (string.Equals(se, "variants", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Variants);
                }

                if (string.Equals(se, "reviews", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Reviews);
                }

            }
        }

        if (!string.IsNullOrWhiteSpace(query.OrderBy))
        {
            var sortBy = query.OrderBy.ToLowerInvariant();

            options = sortBy switch
            {
                "price" => options.AddOrderBy(p => p.Price.Value),
                "brand" => options.AddOrderBy(p => p.Brand.Name),
                "totalsales" => options.AddOrderBy(p => p.TotalSales),
                "totalreviews" => options.AddOrderBy(p => p.TotalReviews),
                "averagerating" => options.AddOrderBy(p => p.AverageRating),
                _ => options
            };
        }

        if (!string.IsNullOrWhiteSpace(query.SortDir))
        {
            var sortDir = query.SortDir.ToLowerInvariant();
            options = sortDir switch
            {
                "asc" => options.AddSortDirAsc(),
                "desc" => options.AddSortDirDesc(),
                _ => options
            };
        }

        return options;
    }
}

