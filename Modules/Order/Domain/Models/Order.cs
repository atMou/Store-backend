using System.ComponentModel.DataAnnotations.Schema;

using Order.Domain.Events;


namespace Order.Domain.Models;

public class Order : Aggregate<OrderId>
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
        CouponIds = couponIds.ToList();
        CartId = cartId;
        TotalAfterDiscounted = totalAfterDiscounted;
        OrderItems = orderItems.ToList();
        TrackingCode = TrackingCode.Create();

    }

    public UserId UserId { get; private init; }
    public CartId CartId { get; private init; }
    public ShipmentId? ShipmentId { get; private set; }
    public PaymentId? PaymentId { get; private set; }
    public string Email { get; private init; }
    public string Phone { get; private set; }
    public decimal Subtotal { get; private init; }
    public decimal Tax { get; private init; }
    public decimal Total { get; private init; }
    public decimal Discount { get; private init; }
    public decimal TotalAfterDiscounted { get; private init; }
    public decimal ShipmentCost { get; private init; }
    public string TransactionId { get; private init; }
    public Address ShippingAddress { get; private set; } = null!;
    public TrackingCode TrackingCode { get; private init; }
    public OrderStatus OrderStatus { get; private set; } = OrderStatus.Pending;
    public ICollection<CouponId> CouponIds { get; private init; }
    public ICollection<OrderItem> OrderItems { get; private init; }

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
            ColorVariantId = item.ColorVariantId,
            SizeVariantId = item.SizeVariantId,
            Slug = item.Slug,
            Sku = item.Sku,
            ImageUrl = item.ImageUrl,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            LineTotal = item.LineTotal,
            Size = item.Size,
            Color = item.Color

        })).AsIterable();

        return items.Traverse(identity).Map(itms =>
        {
            var order = new Order(
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
            );
            return order;
        }).As();
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
        EnsureCanDelete().Map(_ =>
        {
            IsDeleted = true;
            return this;
        });

    public Fin<Order> MarkAsPaid(PaymentId paymentId, DateTime dateTime)
    {
        return OrderStatus.CanTransitionTo(OrderStatus.Paid).Map(_ =>
        {
            var oldStatus = OrderStatus;

            PaymentId = paymentId;
            OrderStatus = OrderStatus.Paid;
            PaidAt = dateTime;

            AddDomainEvent(new OrderStatusChangedDomainEvent(
                Id, oldStatus, OrderStatus.Paid, dateTime));

            AddDomainEvent(new OrderPaidDomainEvent(
                Id, paymentId, dateTime));

            return this;
        });
    }

    public Fin<Order> MarkAsPaymentFailed(DateTime dateTime)
    {
        return OrderStatus.CanTransitionTo(OrderStatus.PaymentFailed).Map(_ =>
        {
            var oldStatus = OrderStatus;
            OrderStatus = OrderStatus.PaymentFailed;
            UpdatedAt = dateTime;

            AddDomainEvent(new OrderStatusChangedDomainEvent(
                Id, oldStatus, OrderStatus.PaymentFailed, dateTime));

            return this;
        });
    }

    public Fin<Order> MarkAsShipped(ShipmentId shipmentId, DateTime dateTime)
    {
        return OrderStatus.CanTransitionTo(OrderStatus.Shipped).Map(_ =>
        {
            var oldStatus = OrderStatus;
            ShipmentId = shipmentId;
            OrderStatus = OrderStatus.Shipped;
            ShippedAt = dateTime;

            AddDomainEvent(new OrderStatusChangedDomainEvent(
                Id, oldStatus, OrderStatus.Shipped, dateTime));

            AddDomainEvent(new OrderShippedDomainEvent(
                Id, shipmentId, dateTime));

            return this;
        });
    }

    public Fin<Order> MarkAsDelivered(DateTime dateTime)
    {
        return OrderStatus.CanTransitionTo(OrderStatus.Delivered).Map(_ =>
        {
            var oldStatus = OrderStatus;
            OrderStatus = OrderStatus.Delivered;
            DeliveredAt = dateTime;

            AddDomainEvent(new OrderStatusChangedDomainEvent(
                Id, oldStatus, OrderStatus.Delivered, dateTime));

            AddDomainEvent(new OrderDeliveredDomainEvent(
                Id, dateTime));

            return this;
        });
    }

    public Fin<Order> MarkAsCancelled(DateTime dateTime)
    {
        return OrderStatus.CanTransitionTo(OrderStatus.Cancelled).Map(_ =>
        {
            var oldStatus = OrderStatus;
            OrderStatus = OrderStatus.Cancelled;
            CancelledAt = dateTime;

            AddDomainEvent(new OrderStatusChangedDomainEvent(
                Id, oldStatus, OrderStatus.Cancelled, dateTime));

            AddDomainEvent(new OrderCancelledDomainEvent(
                Id, dateTime));

            return this;
        });
    }

    public Fin<Order> MarkAsRefunded(DateTime dateTime)
    {
        return OrderStatus.CanTransitionTo(OrderStatus.Refunded).Map(_ =>
        {
            var oldStatus = OrderStatus;
            OrderStatus = OrderStatus.Refunded;
            RefundedAt = dateTime;

            AddDomainEvent(new OrderStatusChangedDomainEvent(
                Id, oldStatus, OrderStatus.Refunded, dateTime));

            return this;
        });
    }

    public Fin<Order> MarkAsCompleted()
    {
        return OrderStatus.CanTransitionTo(OrderStatus.Completed).Map(_ =>
        {
            var oldStatus = OrderStatus;
            OrderStatus = OrderStatus.Completed;

            AddDomainEvent(new OrderStatusChangedDomainEvent(
                Id, oldStatus, OrderStatus.Completed, DateTime.UtcNow));

            return this;
        });
    }

    public Order UpdateShippingAddress(Address newAddress)
    {
        ShippingAddress = newAddress;
        return this;
    }

    public Fin<Order> Update(UpdateOrderDto dto)
    {
        var result = FinSucc(this);

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            var statusDate = dto.StatusDate ?? DateTime.UtcNow;

            result = result.Bind(o => dto.Status.ToLower() switch
            {
                "paid" when dto.PaymentId.HasValue =>
                    o.MarkAsPaid(PaymentId.From(dto.PaymentId.Value), statusDate),
                "shipped" when dto.ShipmentId.HasValue =>
                    o.MarkAsShipped(ShipmentId.From(dto.ShipmentId.Value), statusDate),
                "delivered" => o.MarkAsDelivered(statusDate),
                "cancelled" => o.MarkAsCancelled(statusDate),
                "refunded" => o.MarkAsRefunded(statusDate),
                "completed" => o.MarkAsCompleted(),
                _ => FinSucc(o)
            });
        }

        return result;
    }
}