using Shipment.Domain.Events;
using Shipment.Domain.ValueObjects;

namespace Shipment.Domain.Models;

public class Shipment : Aggregate<ShipmentId>
{
    private Shipment() : base(ShipmentId.New)
    {
    }

    private Shipment(OrderId orderId, Address address, string trackingCode) : base(ShipmentId.New)
    {
        OrderId = orderId;
        ShippingAddress = address;
        TrackingCode = trackingCode;
    }

    public OrderId OrderId { get; private init; }
    public ShippingStatus ShippingStatus { get; private set; } = ShippingStatus.Pending;
    public Address ShippingAddress { get; private set; }
    public string TrackingCode { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public static Shipment Create(OrderId orderId, Address address, string trackingCode)
    {
        var shipment = new Shipment(orderId, address, trackingCode);
        shipment.AddDomainEvent(new ShipmentCreatedDomainEvent(
            shipment.Id,
            orderId,
            trackingCode,
            address
        ));
        return shipment;
    }

    public Fin<Shipment> MarkAsShipped(DateTime date)
    {
        return ShippingStatus.CanTransitionTo(ShippingStatus.Shipped).Map(_ =>
        {
            var oldStatus = ShippingStatus;


            ShippingStatus = ShippingStatus.Shipped;
            ShippedAt = date;


            AddDomainEvent(new ShipmentStatusChangedDomainEvent(
                Id, OrderId, oldStatus, ShippingStatus.Shipped, date));

            AddDomainEvent(new ShipmentShippedDomainEvent(
                Id, OrderId, TrackingCode, date));

            return this;
        });
    }

    public Fin<Shipment> MarkAsDelivered(DateTime date)
    {
        return ShippingStatus.CanTransitionTo(ShippingStatus.Delivered).Map(_ =>
        {
            var oldStatus = ShippingStatus;
            ShippingStatus = ShippingStatus.Delivered;
            DeliveredAt = date;

            AddDomainEvent(new ShipmentStatusChangedDomainEvent(
                Id, OrderId, oldStatus, ShippingStatus.Delivered, date));

            AddDomainEvent(new ShipmentDeliveredDomainEvent(
                Id, OrderId, date));

            return this;
        });
    }

    public Fin<Shipment> PutOnHold()
    {
        return ShippingStatus.CanTransitionTo(ShippingStatus.OnHold).Map(_ =>
        {
            var oldStatus = ShippingStatus;


            ShippingStatus = ShippingStatus.OnHold;

            AddDomainEvent(new ShipmentStatusChangedDomainEvent(
                Id, OrderId, oldStatus, ShippingStatus.OnHold, null));

            return this;
        });
    }

    public Fin<Shipment> Cancel()
    {
        return ShippingStatus.CanTransitionTo(ShippingStatus.Cancelled).Map(_ =>
        {
            var oldStatus = ShippingStatus;
            ShippingStatus = ShippingStatus.Cancelled;

            AddDomainEvent(new ShipmentStatusChangedDomainEvent(
                Id, OrderId, oldStatus, ShippingStatus.Cancelled, null));

            return this;
        });
    }

    public Shipment UpdateAddress(Address newAddress)
    {
        ShippingAddress = newAddress;
        return this;
    }

    public Shipment UpdateTrackingCode(string newTrackingCode)
    {
        TrackingCode = newTrackingCode;
        return this;
    }

    public Fin<Shipment> Update(Domain.Contracts.UpdateShipmentDto dto)
    {
        var result = FinSucc(this);

        // Update address if provided
        if (dto.Address is not null)
        {
            result = result.Map(s => s.UpdateAddress(dto.Address));
        }

        // Update tracking code if provided
        if (!string.IsNullOrWhiteSpace(dto.TrackingCode))
        {
            result = result.Map(s => s.UpdateTrackingCode(dto.TrackingCode));
        }

        // Update status if provided
        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            result = result.Bind(s =>
            {
                var statusDate = dto.StatusDate ?? DateTime.UtcNow;
                return dto.Status.ToLower() switch
                {
                    "shipped" => s.MarkAsShipped(statusDate),
                    "delivered" => s.MarkAsDelivered(statusDate),
                    "onhold" => s.PutOnHold(),
                    "cancelled" => s.Cancel(),
                    _ => FinSucc(s)
                };
            });
        }

        return result;
    }
}
