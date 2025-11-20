using System.ComponentModel.DataAnnotations.Schema;

using Shared.Domain.Errors;

namespace Order.Domain.Models;

public record Order : Aggregate<OrderId>
{
    private Order(UserId userId, IEnumerable<OrderItem> orderItems, Money subtotal, Money total)
        : base(OrderId.New)
    {
        UserId = userId;
        OrderItems = orderItems;
        Subtotal = subtotal;
        Total = total;

        //Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public UserId UserId { get; private init; }
    public IEnumerable<OrderItem> OrderItems { get; private init; }
    public OrderStatus OrderStatus { get; private set; }
    //public PaymentStatus PaymentStatus { get; private set; }
    //public Phone Phone { get; private set; }
    public Address ShippingAddress { get; set; }
    public decimal ShippingCost { get; set; }
    public Email Email { get; private init; }
    public decimal Subtotal { get; private init; }
    public decimal Tax { get; init; }
    public decimal Total { get; private init; }
    public decimal Discount { get; init; }
    //public PaymentMethod PaymentMethod { get; init; }
    //public TrackingCode TrackingCode { get; init; }

    public string Notes { get; init; } = string.Empty;
    public IEnumerable<CouponId> CouponIds { get; init; } = [];

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
    public DateTime? _shippedAt
    {
        get
        {
            return ShippedAt.Match<DateTime?>(
                date => date,
                () => null
            );
        }
        set => ShippedAt = Optional(value);
    }
    public DateTime? _deliveredAt
    {
        get
        {
            return DeliveredAt.Match<DateTime?>(
                date => date,
                () => null
            );
        }
        set => DeliveredAt = Optional(value);
    }
    public DateTime? _cancelledAt
    {
        get
        {
            return CancelledAt.Match<DateTime?>(
                date => date,
                () => null
            );
        }
        set => CancelledAt = Optional(value);
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
    [NotMapped]
    public Option<DateTime> PaidAt { get; private set; }
    [NotMapped]
    public Option<DateTime> DeliveredAt { get; private set; }
    [NotMapped]
    public Option<DateTime> ShippedAt { get; private set; }
    [NotMapped]
    public Option<DateTime> CancelledAt { get; private set; }
    [NotMapped]
    public Option<DateTime> RefundedAt { get; private set; }

    public static Fin<Order> Create(UserId userId, IEnumerable<OrderItem> items)
    {
        var _items = Seq(items);
        if (_items.IsEmpty)
            return FinFail<Order>(InvalidOperationError.New("Order must contain at least one item."));

        var subtotal = _items.Map(i => i.LineTotal).Fold(Money.Zero, (acc, next) => acc + next);
        var total = subtotal;

        return new Order(userId, _items, subtotal, total);
    }

    //public Order MarkAsPaid(DateTime dateTime)
    //{
    //    return this with { Status = OrderStatus.Paid, PaidAt = dateTime };
    //}

    //public Order MarkAsShipped(DateTime dateTime)
    //{
    //    return this with { Status = OrderStatus.Shipped, ShippedAt = dateTime };
    //}

    //public Order MarkAsDelivered(DateTime dateTime)
    //{
    //    return this with { Status = OrderStatus.Delivered, DeliveredAt = dateTime };
    //}

    //public Order MarkAsCancelled(DateTime dateTime)
    //{
    //    return this with { Status = OrderStatus.Cancelled, CancelledAt = dateTime };
    //}
}