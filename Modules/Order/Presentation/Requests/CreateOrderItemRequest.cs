namespace Order.Presentation.Requests;

public record CreateOrderItemRequest
{
	public Guid ProductId { get; init; }
	public string Slug { get; init; }
	public string Sku { get; init; }
	public string ImageUrl { get; init; }
	public int Quantity { get; init; }
	public decimal UnitPrice { get; init; }
}