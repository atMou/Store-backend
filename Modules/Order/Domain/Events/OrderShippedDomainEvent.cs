namespace Order.Domain.Events;

public record OrderShippedDomainEvent(
    OrderId OrderId,
    ShipmentId ShipmentId,
    DateTime ShippedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => nameof(OrderShippedDomainEvent);
}
