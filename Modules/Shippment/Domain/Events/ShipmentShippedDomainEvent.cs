namespace Shipment.Domain.Events;

public record ShipmentShippedDomainEvent(
    ShipmentId ShipmentId,
    OrderId OrderId,
    string TrackingCode,
    DateTime ShippedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => nameof(ShipmentShippedDomainEvent);
}
