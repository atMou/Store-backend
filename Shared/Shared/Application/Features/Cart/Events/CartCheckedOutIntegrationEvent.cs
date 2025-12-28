using Shared.Application.Abstractions;
using Shared.Application.Contracts.Carts.Results;

namespace Shared.Application.Features.Cart.Events;

public record CartCheckedOutIntegrationEvent : IntegrationEvent
{
	public Guid CartId { get; init; }
	public Guid UserId { get; init; }
	public decimal Total { get; init; }
	public decimal TotalSub { get; init; }
	public decimal Tax { get; init; }
	public decimal Discount { get; init; }
	public decimal TotalAfterDiscounted { get; init; }
	public decimal ShipmentCost { get; init; }
	public List<Guid> CouponIds { get; init; }
	public Address DeliveryAddress { get; set; }
	public List<LineItemResult> LineItems { get; init; }
}

