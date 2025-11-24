using System.ComponentModel.DataAnnotations.Schema;

using Identity.Domain.ValueObjects;


namespace Order.Domain.Models;

public record Order : Aggregate<OrderId>
{
    private Order() : base(OrderId.New)
    {
    }
    private Order(UserId userId, IEnumerable<OrderItem> orderItems, Money subtotal, Money total, Money tax, Money discount)
        : base(OrderId.New)
    {
        UserId = userId;
        Subtotal = subtotal;
        Total = total;
        Tax = tax;
        Discount = discount;
        OrderItems = orderItems;
        TrackingCode = TrackingCode.Create();

    }

    public UserId UserId { get; private init; }
    public Email Email { get; private init; }
    public Phone Phone { get; private set; }
    public ShipmentId? ShipmentId { get; private init; }
    public PaymentId? PaymentId { get; private init; }
    public Money Subtotal { get; private init; }
    public Money Tax { get; init; }
    public Money Total { get; private init; }
    public Money Discount { get; init; }
    public TrackingCode TrackingCode { get; init; }
    public OrderStatus OrderStatus { get; private set; } = OrderStatus.Pending;
    public IEnumerable<CouponId> CouponIds { get; init; } = [];
    public IEnumerable<OrderItem> OrderItems { get; private init; }

    public bool IsDeleted { get; private set; }

    public string Notes { get; init; } = string.Empty;

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
    public Option<DateTime> PaidAt { get; private set; } = Option<DateTime>.None;
    [NotMapped]
    public Option<DateTime> DeliveredAt { get; private set; } = Option<DateTime>.None;
    [NotMapped]
    public Option<DateTime> ShippedAt { get; private set; } = Option<DateTime>.None;
    [NotMapped]
    public Option<DateTime> CancelledAt { get; private set; } = Option<DateTime>.None;
    [NotMapped]
    public Option<DateTime> RefundedAt { get; private set; } = Option<DateTime>.None;

    public static Fin<Order> Create(UserId userId, IEnumerable<OrderItem> orderItems, Money subtotal, Money total, Money tax, Money discount)
    {
        var items = orderItems.ToList();
        return !items.Any() ? FinFail<Order>(InvalidOperationError.New("Order must contain at least one item."))
            : new Order(userId, items, subtotal, total, tax, discount);
    }


    public Fin<Unit> EnsureCanDelete()
    {
        if (OrderStatus == OrderStatus.Cancelled || OrderStatus == OrderStatus.Unknown || OrderStatus == OrderStatus.Pending)
        {
            return FinFail<Unit>(InvalidOperationError.New($"Cannot delete order in '{OrderStatus.Name} Status'"));
        }

        return unit;

    }

    public Fin<Order> MarkAsDeleted() =>
        EnsureCanDelete().Map(_ => this with { IsDeleted = true });

    public Fin<Order> MarkAsPaid(PaymentId paymentId, DateTime dateTime) =>
        OrderStatus.CanTransitionTo(OrderStatus.Paid).Map(_ =>
        this with { PaymentId = paymentId, PaidAt = dateTime });


    public Fin<Order> MarkAsShipped(ShipmentId shipmentId, DateTime dateTime) => OrderStatus
        .CanTransitionTo(OrderStatus.Shipped).Map(_ =>
            this with { ShipmentId = shipmentId, ShippedAt = dateTime });

    public Fin<Order> MarkAsDelivered(DateTime dateTime) => OrderStatus.CanTransitionTo(OrderStatus.Delivered).Map(_ =>
        this with { OrderStatus = OrderStatus.Delivered, DeliveredAt = dateTime }
    );
    public Fin<Order> MarkAsCancelled(DateTime dateTime) => OrderStatus.CanTransitionTo(OrderStatus.Cancelled)
        .Map(_ => this with { OrderStatus = OrderStatus.Cancelled, CancelledAt = dateTime });

    public Fin<Order> MarkAsRefunded(DateTime dateTime) => OrderStatus.CanTransitionTo(OrderStatus.Refunded).Map(_ =>
        this with { OrderStatus = OrderStatus.Refunded, RefundedAt = dateTime }
    );


}