namespace Order.Domain.Events;

public record OrderCancelledDomainEvent(
    OrderId OrderId,
    DateTime CancelledAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => nameof(OrderCancelledDomainEvent);
}
