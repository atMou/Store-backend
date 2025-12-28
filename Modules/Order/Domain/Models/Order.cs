using System.ComponentModel.DataAnnotations.Schema;

using Order.Domain.Contracts;


namespace Order.Domain.Models;

public record Order : Aggregate<OrderId>
{
    private Order() : base(OrderId.New)
    {

    }
    private Order(
        UserId userId,
        IEnumerable<OrderItem> orderItems,
        decimal subtotal,
        decimal total,
        decimal tax,
        decimal discount,
        Address address,
        decimal shipmentCost,
        IEnumerable<CouponId> couponIds,
        CartId cartId,
        decimal totalAfterDiscounted)
        : base(OrderId.New)
    {
        UserId = userId;
        Subtotal = subtotal;
        Total = total;
        Tax = tax;
        Discount = discount;
        ShippingAddress = address;
        ShipmentCost = shipmentCost;
        CouponIds = couponIds;
        CartId = cartId;
        TotalAfterDiscounted = totalAfterDiscounted;
        OrderItems = orderItems;
        TrackingCode = TrackingCode.Create();

    }

    public UserId UserId { get; private init; }
    public CartId CartId { get; private init; }
    public ShipmentId? ShipmentId { get; private init; }
    public PaymentId? PaymentId { get; private init; }
    public string Email { get; private init; }
    public string Phone { get; private set; }
    public decimal Subtotal { get; private init; }
    public decimal Tax { get; private init; }
    public decimal Total { get; private init; }
    public decimal Discount { get; private init; }
    public decimal TotalAfterDiscounted { get; private init; }
    public decimal ShipmentCost { get; private init; }
    public string TransactionId { get; private init; }
    public Address ShippingAddress { get; private init; } = null!;
    public TrackingCode TrackingCode { get; private init; }
    public OrderStatus OrderStatus { get; private set; } = OrderStatus.Pending;
    public IEnumerable<CouponId> CouponIds { get; private init; }
    public IEnumerable<OrderItem> OrderItems { get; private init; }

    public bool IsDeleted { get; private set; }

    public string Notes { get; private init; } = string.Empty;

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

    public static Fin<Order> Create(
        CreateOrderDto dto)
    {
        var items = dto.OrderItems.Select(item => OrderItem.Create(new CreateOrderItemDto()
        {
            ProductId = item.ProductId,
            VariantId = item.VariantId,
            Slug = item.Slug,
            Sku = item.Sku,
            ImageUrl = item.ImageUrl,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            LineTotal = item.LineTotal
        })).AsIterable();

        return items.Traverse(identity).Map(itms =>
            new Order(
                dto.UserId,
                itms.AsEnumerable(),
                dto.Subtotal,
                dto.Total,
                dto.Tax,
                dto.Discount,
                dto.DeliveryAddress,
                dto.ShipmentCost,
                dto.CouponIds,
                dto.CartId,
                dto.TotalAfterDiscounted
                )).As();
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