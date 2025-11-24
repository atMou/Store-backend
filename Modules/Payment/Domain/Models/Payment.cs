using Order.Domain.ValueObjects;

using Payment.Domain.ValueObjects;

namespace Payment.Domain.Models;



public record Payment : Aggregate<PaymentId>
{
    private Payment() : base(PaymentId.New)
    {
    }
    private Payment(OrderId orderId, UserId userId) : base(PaymentId.New)
    {
        OrderId = orderId;
        UserId = userId;
    }
    public OrderId OrderId { get; private init; }
    public UserId UserId { get; private init; }
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; private set; } = PaymentMethod.Unknown;
    public string TransactionId { get; private set; }
    public DateTime? _paidAt
    {
        get
        {
            return PaidAt.Match<DateTime?>(
                date => date,
                () => null
            );
        }
        set => PaidAt = Optional(value);
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
    public Option<DateTime> PaidAt { get; private set; } = Option<DateTime>.None;
    public Option<DateTime> RefundedAt { get; private set; } = Option<DateTime>.None;

    public static Payment Create(OrderId orderId, UserId userId)
    {
        return new Payment(orderId, userId);
    }
    public Fin<Payment> MarkAsPaid(PaymentMethod method, string transactionId, DateTime date) =>
        PaymentStatus.EnsureCanTransitionTo(PaymentStatus.Paid).Map(_ => this with
        {
            PaymentStatus = PaymentStatus.Paid,
            PaymentMethod = method,
            TransactionId = transactionId,
            PaidAt = date
        });



    public Fin<Payment> MarkAsFailed(PaymentMethod method, DateTime date) =>
        PaymentStatus.EnsureCanTransitionTo(PaymentStatus.Failed).Map(_ => this with
        {
            PaymentStatus = PaymentStatus.Failed,
            PaymentMethod = method,
        });


    public Fin<Payment> MarkAsRefund(PaymentMethod method, DateTime date) =>
        PaymentStatus.EnsureCanTransitionTo(PaymentStatus.Paid).Map(_ => this with
        {
            PaymentStatus = PaymentStatus.Refunded,
            PaymentMethod = method,
            RefundedAt = date
        });

}
