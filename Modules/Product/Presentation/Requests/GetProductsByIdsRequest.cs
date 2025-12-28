namespace Product.Presentation.Requests;

public record GetProductsByIdsRequest
{

    public IEnumerable<Guid> ProductIds { get; init; }
    public string? OrderBy { get; init; }
    public string? SortDir { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public string? Include { get; init; }

    public GetProductsByIdsQuery ToQuery()
    {
        return new GetProductsByIdsQuery
        {
            ProductIds = ProductIds.Select(id => ProductId.From(id)),
            OrderBy = OrderBy,
            SortDir = SortDir,
            PageNumber = PageNumber,
            PageSize = PageSize,
            Include = Include
        };
    }
}