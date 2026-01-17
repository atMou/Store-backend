namespace Order.Domain.Events;

public record OrderStatusChangedDomainEvent(
    OrderId OrderId,
    OrderStatus OldStatus,
    OrderStatus NewStatus,
    DateTime ChangedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => nameof(OrderStatusChangedDomainEvent);
}
