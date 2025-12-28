using Product.Application.Features.GetProducts;

namespace Product.Presentation.Requests;

public record GetProductsRequest
{
	public string? Category { get; init; }
	public string? Brand { get; init; }
	public string? Color { get; init; }
	public string? Type { get; init; }
	public string? Sub { get; init; }
	public string? Size { get; init; }
	public decimal? MinPrice { get; init; }
	public decimal? MaxPrice { get; init; }
	public string? Search { get; init; }
	public string? OrderBy { get; init; }
	public string? SortDir { get; init; }
	public int? PageNumber { get; init; }
	public int? PageSize { get; init; }
	public string? Include { get; init; }
	public bool? IsFeatured { get; init; }
	public bool? IsTrending { get; init; }
	public bool? IsBestSeller { get; init; }
	public bool? IsNew { get; init; }


	public GetProductsQuery ToQuery() => new()
	{
		Category = Category,
		Brand = Brand,
		Color = Color,
		Type = Type,
		Sub = Sub,
		Size = Size,
		MinPrice = MinPrice,
		MaxPrice = MaxPrice,
		Search = Search,
		OrderBy = OrderBy,
		SortDir = SortDir,
		PageNumber = PageNumber ?? 1,
		PageSize = PageSize ?? 20,
		Include = Include,
		IsFeatured = IsFeatured,
		IsTrending = IsTrending,
		IsBestSeller = IsBestSeller,
		IsNew = IsNew
	};
}
