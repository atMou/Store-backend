namespace Payment.Domain.Events;

public record PaymentCancelledEvent : IDomainEvent
{
    public PaymentId PaymentId { get; init; }
    public OrderId OrderId { get; init; }
    public UserId UserId { get; init; }
    public CartId CartId { get; init; }
    public DateTime CancelledAt { get; init; }
}
