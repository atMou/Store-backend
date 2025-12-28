using Shipment.Domain.ValueObjects;

namespace Shipment.Domain.Models;

public record Shipment : Aggregate<ShipmentId>
{

	private Shipment(OrderId orderId, Address address, string trackingCode) : base(ShipmentId.New)
	{
		OrderId = orderId;
		ShippingAddress = address;
		TrackingCode = trackingCode;
	}

	public OrderId OrderId { get; private init; }
	public ShippingStatus ShippingStatus { get; private init; } = ShippingStatus.Pending;
	public Address ShippingAddress { get; private set; }
	public string TrackingCode { get; private set; }


	public DateTime? ShippedAt { get; private set; }

	public DateTime? DeliveredAt { get; private set; }

	public static Shipment Create(OrderId orderId, Address address, string trackingCode) =>
		new Shipment(orderId, address, trackingCode);

	public Fin<Shipment> MarkAsShipped(DateTime date) =>
		ShippingStatus.CanTransitionTo(ShippingStatus.Shipped).Map(_ => this with
		{
			ShippingStatus = ShippingStatus.Shipped,
			ShippedAt = date,
		});

	public Fin<Shipment> MarkAsDelivered(DateTime date) =>
		ShippingStatus.CanTransitionTo(ShippingStatus.Delivered).Map(_ => this with
		{
			ShippingStatus = ShippingStatus.Delivered,
			DeliveredAt = date,

		});



	public Fin<Shipment> PutOnHold() =>
		ShippingStatus.CanTransitionTo(ShippingStatus.OnHold).Map(_ => this with
		{
			ShippingStatus = ShippingStatus.OnHold,

		});

	public Fin<Shipment> Cancel() =>
	ShippingStatus.CanTransitionTo(ShippingStatus.Cancelled).Map(_ => this with
	{
		ShippingStatus = ShippingStatus.Cancelled,

	});

}
