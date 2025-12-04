using Shared.Application.Contracts.Product.Results;

namespace Shared.Application.Contracts.Product.Queries;
public record GetProductsByIdsQuery : IQuery<Fin<PaginatedResult<ProductResult>>>, IPagination, IInclude
{
    public IEnumerable<ProductId> ProductIds { get; init; } = [];
    public string? OrderBy { get; init; }
    public string? SortDir { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public string? Include { get; init; }
}
