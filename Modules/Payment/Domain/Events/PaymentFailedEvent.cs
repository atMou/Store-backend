namespace Payment.Domain.Events;

public record PaymentFailedEvent : IDomainEvent
{
    public PaymentId PaymentId { get; init; }
    public OrderId OrderId { get; init; }
    public UserId UserId { get; init; }
    public CartId CartId { get; init; }
    public string PaymentTransactionId { get; init; }
    public DateTime PaymentFailedAt { get; init; }
}