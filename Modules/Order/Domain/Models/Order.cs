namespace Order.Domain.Models;

public record Order : Aggregate<OrderId>
{
    private Order(UserId userId, Seq<OrderItem> items, Money subtotal, Money total, Currency currency)
        : base(OrderId.New)
    {
        UserId = userId;
        Items = items;
        Subtotal = subtotal;
        Total = total;
        Currency = currency;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public UserId UserId { get; private init; }
    public Seq<OrderItem> Items { get; private init; }
    public Money Subtotal { get; private init; }
    public Money Total { get; private init; }
    public Currency Currency { get; private init; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private init; }

    public Option<DateTime> PaidAt { get; private set; }
    public Option<DateTime> ShippedAt { get; private set; }
    public Option<DateTime> DeliveredAt { get; private set; }

    public static Fin<Order> Create(UserId userId, Seq<OrderItem> items, Currency currency)
    {
        if (items.IsEmpty)
            return FinFail<Order>(Error.New("Order must contain at least one item."));

        var subtotal = items.Map(i => i.LineTotal).Fold(Money.Zero, (acc, next) => acc + next);
        var total = subtotal; // You could apply discounts/taxes here

        return new Order(userId, items, subtotal, total, currency);
    }

    public Order MarkAsPaid() =>
        this with { Status = OrderStatus.Paid, PaidAt = DateTime.UtcNow };

    public Order MarkAsShipped() =>
        this with { Status = OrderStatus.Shipped, ShippedAt = DateTime.UtcNow };

    public Order MarkAsDelivered() =>
        this with { Status = OrderStatus.Delivered, DeliveredAt = DateTime.UtcNow };
}

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Delivered,
    Cancelled
}