namespace Order.Domain.Events;

public record OrderDeliveredDomainEvent(
    OrderId OrderId,
    DateTime DeliveredAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => nameof(OrderDeliveredDomainEvent);
}
