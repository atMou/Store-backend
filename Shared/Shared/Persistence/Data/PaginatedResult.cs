namespace Shared.Persistence.Data;
public record PaginatedResult<T>
{
    public IEnumerable<T> Items { get; init; }
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}

