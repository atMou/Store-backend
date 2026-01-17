namespace Shipment.Domain.Events;

public record ShipmentCreatedDomainEvent(
    ShipmentId ShipmentId,
    OrderId OrderId,
    string TrackingCode,
    Address ShippingAddress) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => nameof(ShipmentCreatedDomainEvent);
}
