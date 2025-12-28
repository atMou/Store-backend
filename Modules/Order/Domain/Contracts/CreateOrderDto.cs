namespace Order.Domain.Contracts;

public record CreateOrderDto
{
	public CartId CartId { get; init; }
	public UserId UserId { get; init; }
	public decimal Total { get; init; }
	public decimal Subtotal { get; init; }
	public decimal Tax { get; init; }
	public decimal Discount { get; init; }
	public decimal TotalAfterDiscounted { get; init; }
	public Address DeliveryAddress { get; init; }
	public decimal ShipmentCost { get; init; }

	public IEnumerable<CouponId> CouponIds { get; init; }
	public IEnumerable<CreateOrderItemDto> OrderItems { get; init; }
}