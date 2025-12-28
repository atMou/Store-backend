using Shared.Application.Contracts.Order.Dtos;

namespace Order.Domain.Events;

public record OrderCreatedDomainEvent : IDomainEvent
{
	public Guid OrderId { get; init; }
	public Guid CartId { get; init; }
	public Guid UserId { get; init; }
	public decimal Total { get; init; }
	public decimal Subtotal { get; init; }
	public decimal Tax { get; init; }
	public decimal Discount { get; init; }
	public decimal TotalAfterDiscounted { get; init; }
	public Address Address { get; init; }

	public decimal ShipmentCost { get; init; }

	public IEnumerable<Guid> CouponIds { get; init; }
	public IEnumerable<OrderItemDto> OrderItemsDtos { get; init; }

}