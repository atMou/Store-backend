namespace Payment.Domain.Events;

public record PaymentRefundedEvent : IDomainEvent
{
    public PaymentId PaymentId { get; init; }
    public OrderId OrderId { get; init; }
    public UserId UserId { get; init; }
    public DateTime PaymentRefundedAt { get; init; }
}