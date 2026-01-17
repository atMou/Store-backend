using System.ComponentModel.DataAnnotations.Schema;

using Order.Domain.ValueObjects;

using Payment.Domain.Events;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Models;

public class Payment : Aggregate<PaymentId>
{
    private Payment() : base(PaymentId.New)
    {
    }
    private Payment(OrderId orderId, UserId userId, CartId cartId, decimal tax, decimal total) : base(PaymentId.New)
    {
        OrderId = orderId;
        UserId = userId;
        CartId = cartId;
        Tax = tax;
        Total = total;
    }
    public OrderId OrderId { get; private init; }
    public UserId UserId { get; private init; }
    public CartId CartId { get; private init; }
    public decimal Tax { get; private init; }
    public decimal Total { get; private init; }
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; private set; } = PaymentMethod.Unknown;
    public string? TransactionId { get; private set; }
    public string? RefundId { get; private set; }
    [NotMapped]
    public string? Currency { get; private set; }
    [NotMapped]
    public string? ClientSecret { get; private set; }
    [NotMapped]
    public decimal Amount { get; private set; }
    public decimal? RefundAmount { get; private set; }

    public string? RefundStatus { get; private set; }

    public DateTime? PaidAt { get; private set; }
    public DateTime? RefundedAt { get; private set; }
    public DateTime? FailedAt { get; private set; }

    public static Payment Create(OrderId orderId, UserId userId, CartId cartId, decimal total, decimal tax)
    {
        return new Payment(orderId, userId, cartId, tax, total);
    }

    public Payment SetStripePaymentIntentId(string stripePaymentIntentId, decimal amount, string currency, string clientSecret)
    {
        TransactionId = stripePaymentIntentId;
        Amount = amount;
        ClientSecret = clientSecret;
        Currency = currency;

        return this;
    }

    public Fin<Payment> EnsureCanMakePayment(PaymentMethod method) =>
        PaymentStatus.EnsureCanTransitionTo(PaymentStatus.Processing).Map(_ =>
        {
            PaymentMethod = method;
            PaymentStatus = PaymentStatus.Processing;
            return this;
        });

    public Fin<Payment> EnsureCanCancel() =>
        PaymentStatus.EnsureCanTransitionTo(PaymentStatus.Cancelled)
            .Bind(_ => TransactionId is null
                ? FinFail<Unit>(InvalidOperationError.New("Payment does not have a transaction ID"))
                : FinSucc(unit))
            .Map(_ => this);

    public Fin<Payment> MarkAsCancelled(DateTime date) =>
        EnsureCanCancel().Map(_ =>
        {
            PaymentStatus = PaymentStatus.Cancelled;
            FailedAt = date;
            return this;
        }).Map(payment =>
        {
            AddDomainEvent(new PaymentCancelledEvent
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                UserId = payment.UserId,
                CartId = payment.CartId,
                CancelledAt = date
            });
            return payment;
        });

    public Fin<Payment> EnsureCanRefund() =>
        PaymentStatus.EnsureCanTransitionTo(PaymentStatus.Refunded)
            .Bind(_ => TransactionId is null
                ? FinFail<Unit>(InvalidOperationError.New("Payment does not have a transaction ID"))
                : FinSucc(unit))
            .Map(_ => this);

    public Fin<Payment> MarkAsFulfilled(PaymentMethod paymentMethod, DateTime date) =>
        PaymentStatus.EnsureCanTransitionTo(PaymentStatus.Paid).Map(_ =>
        {
            PaymentStatus = PaymentStatus.Paid;
            PaymentMethod = paymentMethod;
            PaidAt = date;
            return this;
        }).Map(payment =>
        {
            AddDomainEvent(new PaymentFulfilledEvent
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                UserId = payment.UserId,
                CartId = payment.CartId,
                PaymentTransactionId = payment.TransactionId!,
                PaymentPaidAt = payment.PaidAt,
            });
            return payment;
        });

    public Fin<Payment> MarkAsFailed(PaymentMethod paymentMethod, DateTime date) =>
        PaymentStatus.EnsureCanTransitionTo(PaymentStatus.Failed).Map(_ =>
        {
            PaymentStatus = PaymentStatus.Failed;
            PaymentMethod = paymentMethod;
            FailedAt = date;
            return this;
        }).Map(payment =>
        {
            AddDomainEvent(new PaymentFailedEvent
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                UserId = payment.UserId,
                CartId = payment.CartId,
            });
            return payment;
        });

    public Fin<Payment> MarkAsRefunded(PaymentMethod paymentMethod, DateTime date, decimal refundAmount, string status, string refundId) =>
        EnsureCanRefund().Map(_ =>
        {
            RefundAmount = refundAmount;
            RefundStatus = status;
            RefundId = refundId;
            PaymentMethod = paymentMethod;
            PaymentStatus = PaymentStatus.Refunded;
            RefundedAt = date;
            return this;

        }).Map(payment =>
        {
            AddDomainEvent(new PaymentRefundedEvent
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                UserId = payment.UserId,
                PaymentRefundedAt = payment.RefundedAt
            });
            return payment;
        });
}