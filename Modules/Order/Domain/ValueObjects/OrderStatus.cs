namespace Order.Domain.ValueObjects;

public record OrderStatus
{
    private static readonly List<OrderStatus> _all = new();
    private List<OrderStatus> AllowedStatusChangeTo { get; set; } = new();

    private OrderStatus() { }

    private OrderStatus(OrderStatusCode code, string name, string description)
    {
        Code = code;
        Name = name;
        Description = description;
        _all.Add(this);
    }

    public OrderStatusCode Code { get; }
    public string Name { get; }
    public string Description { get; }

    public static IReadOnlyList<OrderStatus> All => _all;

    public static readonly OrderStatus Pending =
        new(OrderStatusCode.Pending, nameof(Pending), "Order created, awaiting payment.");

    public static readonly OrderStatus PaymentFailed =
        new(OrderStatusCode.PaymentFailed, nameof(PaymentFailed), "Payment attempt failed.");

    public static readonly OrderStatus Paid =
        new(OrderStatusCode.Paid, nameof(Paid), "Payment received and confirmed.");

    public static readonly OrderStatus Processing =
        new(OrderStatusCode.Processing, nameof(Processing), "Order is being prepared for shipment.");

    public static readonly OrderStatus Shipped =
        new(OrderStatusCode.Shipped, nameof(Shipped), "Order has been shipped to the customer.");

    public static readonly OrderStatus Delivered =
        new(OrderStatusCode.Delivered, nameof(Delivered), "Order delivered to the customer.");

    public static readonly OrderStatus Completed =
        new(OrderStatusCode.Completed, nameof(Completed), "Order completed successfully.");

    public static readonly OrderStatus Cancelled =
        new(OrderStatusCode.Cancelled, nameof(Cancelled), "Order cancelled before shipment."
        );

    public static readonly OrderStatus Refunded =
        new(OrderStatusCode.Refunded, nameof(Refunded), "Order payment refunded.");

    public static readonly OrderStatus Returned =
        new(OrderStatusCode.Returned, nameof(Returned), "Customer returned the order.");

    public static readonly OrderStatus OnHold =
        new(OrderStatusCode.OnHold, nameof(OnHold), "Order is temporarily paused.");

    public static readonly OrderStatus Unknown =
        new(OrderStatusCode.Unknown, nameof(Unknown), "Unknown status.");

    static OrderStatus()
    {
        _ = Pending;
        _ = PaymentFailed;
        _ = Paid;
        _ = Processing;
        _ = Shipped;
        _ = Delivered;
        _ = Completed;
        _ = Cancelled;
        _ = Refunded;
        _ = Returned;
        _ = OnHold;
        _ = Unknown;

        Pending.AllowedStatusChangeTo = [PaymentFailed, Processing, Paid, Cancelled, OnHold];
        PaymentFailed.AllowedStatusChangeTo = [Pending, OnHold];
        Paid.AllowedStatusChangeTo = [Shipped, Delivered];
        Processing.AllowedStatusChangeTo = [Shipped];
        Shipped.AllowedStatusChangeTo = [Delivered];
        Delivered.AllowedStatusChangeTo = [Completed];
        Completed.AllowedStatusChangeTo = [Returned];
        OnHold.AllowedStatusChangeTo = [Pending, Processing, Cancelled];
        Refunded.AllowedStatusChangeTo = [];
        Cancelled.AllowedStatusChangeTo = [];
        Unknown.AllowedStatusChangeTo = [];
    }

    public Fin<Unit> CanTransitionTo(OrderStatus target) =>

        AllowedStatusChangeTo.Contains(target) ? FinSucc(unit)
            : FinFail<Unit>(InvalidOperationError.New($"Cannot transition from '{Name}' to '{target.Name}'."));

    public static Fin<OrderStatus> FromCode(string code) =>
        Enum.TryParse<OrderStatusCode>(code, true, out var statusCode)
            ? Optional(_all.FirstOrDefault(s => s.Code == statusCode))
                .ToFin(InvalidOperationError.New($"Invalid order status code '{code}'"))
            : FinFail<OrderStatus>(InvalidOperationError.New($"Invalid order status code '{code}'"));

    public static Fin<OrderStatus> FromName(string name) =>
        Optional(_all.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            .ToFin(InvalidOperationError.New($"Invalid order status name '{name}'"));

    public static OrderStatus FromUnsafe(string repr) =>
        _all.FirstOrDefault(s => s.Name.Equals(repr, StringComparison.OrdinalIgnoreCase)) ?? Unknown;


    public virtual bool Equals(OrderStatus? other)
    {
        return other is { } o && Code == o.Code;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Code, Name);
    }
}

