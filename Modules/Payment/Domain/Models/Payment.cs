using Order.Domain.ValueObjects;

using Payment.Domain.Events;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Models;



public record Payment : Aggregate<PaymentId>
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
    public PaymentStatus PaymentStatus { get; private init; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; private init; } = PaymentMethod.Unknown;
    public string TransactionId { get; private init; }
    public DateTime PaidAt { get; private init; }
    public DateTime RefundedAt { get; private init; }

    public static Payment Create(OrderId orderId, UserId userId, CartId cartId, decimal total, decimal tax)
    {
        return new Payment(orderId, userId, cartId, tax, total);
    }
    public Fin<Payment> MarkAsFulfilled(string method, string transactionId, DateTime date) =>
      PaymentMethod.From(method).Bind(paymentMethod =>
          PaymentStatus.EnsureCanTransitionTo(PaymentStatus.Paid).Map(_ => this with
          {
              PaymentStatus = PaymentStatus.Paid,
              PaymentMethod = paymentMethod,
              TransactionId = transactionId,
              PaidAt = date
          }).Map(payment =>
          {
              AddDomainEvent(new PaymentFulfilledEvent
              {
                  PaymentId = payment.Id,
                  OrderId = payment.OrderId,
                  UserId = payment.UserId,
                  CartId = payment.CartId,

                  PaymentTransactionId = payment.TransactionId,
                  PaymentPaidAt = payment.PaidAt,
              });
              return payment;
          })
      );

    public Fin<Payment> MarkAsFailed(string method, string transactionId, DateTime date) =>
        PaymentMethod.From(method).Bind(paymentMethod =>
            PaymentStatus.EnsureCanTransitionTo(PaymentStatus.Failed).Map(_ => this with
            {
                PaymentStatus = PaymentStatus.Failed,
                PaymentMethod = paymentMethod,
                TransactionId = transactionId,
                PaidAt = date
            })
        ).Map(payment =>
        {
            AddDomainEvent(new PaymentFailedEvent
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                UserId = payment.UserId,
                CartId = payment.CartId,
                PaymentTransactionId = payment.TransactionId,
                PaymentFailedAt = payment.PaidAt
            });
            return payment;
        });


    public Fin<Payment> MarkAsRefund(string method, DateTime date) =>
        PaymentMethod.From(method).Bind(paymentMethod =>
            PaymentStatus.EnsureCanTransitionTo(PaymentStatus.Refunded).Map(_ => this with
            {
                PaymentStatus = PaymentStatus.Refunded,
                PaymentMethod = paymentMethod,
                RefundedAt = date
            })
        ).Map(payment =>
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