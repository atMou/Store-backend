using Product.Application.Features.GetProducts;

namespace Product.Presentation.Requests;

public record GetProductsRequest
{
    public string? Category { get; init; }
    public string? Brand { get; init; }
    public string? Color { get; init; }
    public string? Size { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public string? Search { get; init; }
    public bool? IncludeVariants { get; init; }
    public bool? IncludeReviews { get; init; }
    public string? OrderBy { get; init; }
    public string? SortDir { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public GetProductsQuery ToQuery() => new()
    {
        Category = Category,
        Brand = Brand,
        Color = Color,
        Size = Size,
        MinPrice = MinPrice,
        MaxPrice = MaxPrice,
        Search = Search,
        IncludeVariants = IncludeVariants,
        IncludeReviews = IncludeReviews,
        OrderBy = OrderBy,
        SortDir = SortDir,
        PageNumber = PageNumber,
        PageSize = PageSize
    };
}
