namespace Product.Persistence.Data;
public record PaginatedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize
);

