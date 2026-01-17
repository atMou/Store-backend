using Shipment.Domain.ValueObjects;

namespace Shipment.Domain.Events;

public record ShipmentStatusChangedDomainEvent(
    ShipmentId ShipmentId,
    OrderId OrderId,
    ShippingStatus OldStatus,
    ShippingStatus NewStatus,
    DateTime? StatusChangedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => nameof(ShipmentStatusChangedDomainEvent);
}
