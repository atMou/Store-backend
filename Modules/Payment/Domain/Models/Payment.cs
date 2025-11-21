using Payment.Domain.ValueObjects;

namespace Payment.Domain.Models;

using Order.Domain.ValueObjects; // Only for OrderId reference


public record Payment : Aggregate<PaymentId>
{
    private Payment() : base(PaymentId.New)
    {
    }
    private Payment(OrderId orderId) : base(PaymentId.New)
    {
        OrderId = orderId;
    }
    public OrderId OrderId { get; private init; } // only a reference
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; private set; } = PaymentMethod.Unknown;

    private string? _transactionId
    {
        get
        {
            return TransactionId.Match<string?>(
                date => date,
                () => null
            );
        }
        set => TransactionId = Optional(value);
    }
    public DateTime? _refundedAt
    {
        get
        {
            return RefundedAt.Match<DateTime?>(
                date => date,
                () => null
            );
        }
        set => RefundedAt = Optional(value);
    }
    public Option<string> TransactionId { get; private set; } = Option<string>.None;
    public Option<DateTime> PaidAt { get; private set; } = Option<DateTime>.None;
    public Option<DateTime> RefundedAt { get; private set; } = Option<DateTime>.None;

    public static Payment Create(OrderId orderId)
    {
        return new Payment(orderId);
    }
    public Fin<Payment> MarkAsPaid(PaymentMethod method, string transactionId, DateTime date) =>
        PaymentStatus.CanTransitionTo(PaymentStatus.Paid).Map(_ => this with
        {
            PaymentStatus = PaymentStatus.Paid,
            PaymentMethod = method,
            TransactionId = transactionId,
            PaidAt = date
        });



    public Fin<Payment> MarkAsFailed(PaymentMethod method, DateTime date) =>
        PaymentStatus.CanTransitionTo(PaymentStatus.Failed).Map(_ => this with
        {
            PaymentStatus = PaymentStatus.Failed,
            PaymentMethod = method,
        });


    public Fin<Payment> MarkAsRefund(PaymentMethod method, DateTime date) =>
        PaymentStatus.CanTransitionTo(PaymentStatus.Paid).Map(_ => this with
        {
            PaymentStatus = PaymentStatus.Refunded,
            PaymentMethod = method,
            RefundedAt = date
        });

}
