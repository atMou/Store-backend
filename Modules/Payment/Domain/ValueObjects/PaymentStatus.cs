using Payment.Domain.Enums;

namespace Payment.Domain.ValueObjects;

public record PaymentStatus
{
    private static readonly List<PaymentStatus> _all = new();
    private List<PaymentStatus> AllowedStatusChangeTo { get; set; } = new();

    private PaymentStatus() { }

    private PaymentStatus(PaymentStatusCode code, string name, string description)
    {
        Code = code;
        Name = name;
        Description = description;
        _all.Add(this);
    }

    public PaymentStatusCode Code { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; }

    public static IReadOnlyList<PaymentStatus> All => _all;

    // Predefined payment statuses
    public static readonly PaymentStatus Pending =
        new(PaymentStatusCode.Pending, nameof(Pending), "PaymentInfo is pending.");

    public static readonly PaymentStatus Failed =
        new(PaymentStatusCode.Failed, nameof(Failed), "PaymentInfo attempt failed.");

    public static readonly PaymentStatus Authorized =
        new(PaymentStatusCode.Authorized, nameof(Authorized), "PaymentInfo has been authorized but not captured.");

    public static readonly PaymentStatus Paid =
        new(PaymentStatusCode.Paid, nameof(Paid), "PaymentInfo successfully completed.");

    public static readonly PaymentStatus Refunded =
        new(PaymentStatusCode.Refunded, nameof(Refunded), "PaymentInfo has been refunded.");

    public static readonly PaymentStatus Voided =
        new(PaymentStatusCode.Voided, nameof(Voided), "PaymentInfo has been voided/cancelled.");

    public static readonly PaymentStatus Unknown =
        new(PaymentStatusCode.Unknown, nameof(Unknown), "Unknown payment status.");

    static PaymentStatus()
    {
        _ = Pending;
        _ = Failed;
        _ = Authorized;
        _ = Paid;
        _ = Refunded;
        _ = Voided;
        _ = Unknown;

        Pending.AllowedStatusChangeTo = [Authorized, Paid, Failed, Voided];
        Authorized.AllowedStatusChangeTo = [Paid, Voided];
        Paid.AllowedStatusChangeTo = [Refunded];
        Failed.AllowedStatusChangeTo = [Pending];
        Voided.AllowedStatusChangeTo = [];
        Refunded.AllowedStatusChangeTo = [];
        Unknown.AllowedStatusChangeTo = [];
    }

    public Fin<Unit> EnsureCanTransitionTo(PaymentStatus target) =>

        AllowedStatusChangeTo.Contains(target)
            ? unit
            : FinFail<Unit>(
                InvalidOperationError.New($"Changing Payment status from '{Name}' to '{target.Name}' is not allowed"));

    public static PaymentStatus FromUnsafe(string repr) =>
        _all.FirstOrDefault(s => s.Name.Equals(repr, StringComparison.OrdinalIgnoreCase)) ?? Unknown;
}
