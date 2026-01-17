namespace Order.Domain.Events;

public record OrderPaidDomainEvent(
    OrderId OrderId,
    PaymentId PaymentId,
    DateTime PaidAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => nameof(OrderPaidDomainEvent);
}
