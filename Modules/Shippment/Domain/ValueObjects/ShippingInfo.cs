namespace Shipment.Domain.ValueObjects;

public record ShippingInfo
{
	public ShippingStatus Status { get; private set; } = ShippingStatus.Pending;
	public Address Address { get; private set; }
	public Option<TrackingCode> TrackingCode { get; private set; } = Option<TrackingCode>.None;
	public Option<DateTime> ShippedAt { get; private set; } = Option<DateTime>.None;
	public Option<DateTime> DeliveredAt { get; private set; } = Option<DateTime>.None;

	public ShippingInfo(Address address) => Address = address;

	public ShippingInfo MarkAsShipped(DateTime date, TrackingCode tracking)
	{
		if (!Status.CanTransitionTo(ShippingStatus.Shipped).IsSucc)
		{
			throw new InvalidOperationException($"Cannot ship when status is {Status.Name}");
		}

		return this with
		{
			Status = ShippingStatus.Shipped,
			ShippedAt = date,
			TrackingCode = tracking
		};
	}

	public ShippingInfo MarkAsDelivered(DateTime date)
	{
		if (!Status.CanTransitionTo(ShippingStatus.Delivered).IsSucc)
		{
			throw new InvalidOperationException($"Cannot deliver when status is {Status.Name}");
		}

		return this with
		{
			Status = ShippingStatus.Delivered,
			DeliveredAt = date
		};
	}

	public ShippingInfo PutOnHold()
	{
		if (!Status.CanTransitionTo(ShippingStatus.OnHold).IsSucc)
		{
			throw new InvalidOperationException($"Cannot put on hold when status is {Status.Name}");
		}

		return this with { Status = ShippingStatus.OnHold };
	}

	public ShippingInfo Cancel()
	{
		if (!Status.CanTransitionTo(ShippingStatus.Cancelled).IsSucc)
		{
			throw new InvalidOperationException($"Cannot cancel when status is {Status.Name}");
		}

		return this with { Status = ShippingStatus.Cancelled };
	}
}
