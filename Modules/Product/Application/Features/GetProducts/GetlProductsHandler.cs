using Product.Domain.Contracts;
using Product.Persistence.Data;
using Product.Persistence.Extensions;

using Shared.Application.Abstractions;
using Shared.Domain.Contracts.Product;

namespace Product.Application.Features.GetProducts;

internal abstract record GetProductQuery(
    string? Category,
    string? Brand,
    string? Color,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? Search,
    bool? includeVariants,
    bool? includeReviews,
    string? SortBy = "CreatedAt",
    string? SortDir = "desc",
    int Page = 1,
    int PageSize = 20
) : IQuery<Fin<PaginatedResult<ProductDto>>>;

internal record GetProductsResult(IEnumerable<ProductDto> products);

internal class GetProductsQueryHandler(ProductDBContext dbContext)
    : IQueryHandler<GetProductQuery, Fin<PaginatedResult<ProductDto>>>
{
    public Task<Fin<PaginatedResult<ProductDto>>> Handle(GetProductQuery query, CancellationToken cancellationToken)
    {
        return (
            from list in Db<ProductDBContext>.liftIO(async (ctx, e) =>
                await ctx.Products.GetQueryable(query).ToListAsync(e.Token))
            select Project(list, query)
        ).RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private static PaginatedResult<ProductDto> Project(IEnumerable<Domain.Models.Product> enumerable,
        GetProductQuery query)
    {
        var arr = enumerable as Domain.Models.Product[] ?? enumerable.ToArray();
        return new PaginatedResult<ProductDto>(arr.Select(p => p.ToDto()), arr.Length, query.Page, query.PageSize);
    }
}