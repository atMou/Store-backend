using Shared.Persistence.Data;

namespace Product.Application.Features.GetProducts;

public record GetProductsQuery : IQuery<Fin<PaginatedResult<ProductDto>>>
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

public record GetProductsQueryResult(PaginatedResult<ProductDto> PaginatedResult);

internal class GetProductsQueryHandler(ProductDBContext dbContext, IProductRepository productRepository)
    : IQueryHandler<GetProductsQuery, Fin<PaginatedResult<ProductDto>>>
{
    public Task<Fin<PaginatedResult<ProductDto>>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        return (
            from res in Db<ProductDBContext>.liftIO(ctx => productRepository
                .GetProducts(ctx, query))
            select new PaginatedResult<ProductDto>
            {
                Items = res.Products.ToDto(),
                TotalCount = res.TotalCount,
                PageSize = query.PageSize,
                PageNumber = query.PageNumber
            }
        ).RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

}
