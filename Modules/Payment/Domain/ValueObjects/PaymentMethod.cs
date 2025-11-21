using Order.Domain.Enums;

using Shared.Domain.Errors;

namespace Order.Domain.ValueObjects;

public record PaymentMethod
{
    private static readonly List<PaymentMethod> _all = new();

    private PaymentMethod(PaymentMethodCode code, string name, string description)
    {
        Code = code;
        Name = name;
        Description = description;
        _all.Add(this);
    }

    public PaymentMethodCode Code { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; }

    public static IReadOnlyList<PaymentMethod> All => _all;

    // Predefined payment methods
    public static readonly PaymentMethod CreditCard =
        new(PaymentMethodCode.CreditCard, nameof(CreditCard), "PaymentInfo using credit card.");

    public static readonly PaymentMethod DebitCard =
        new(PaymentMethodCode.DebitCard, nameof(DebitCard), "PaymentInfo using debit card.");

    public static readonly PaymentMethod PayPal =
        new(PaymentMethodCode.PayPal, nameof(PayPal), "PaymentInfo via PayPal.");

    public static readonly PaymentMethod BankTransfer =
        new(PaymentMethodCode.BankTransfer, nameof(BankTransfer), "PaymentInfo via bank transfer.");

    public static readonly PaymentMethod CashOnDelivery =
        new(PaymentMethodCode.CashOnDelivery, nameof(CashOnDelivery), "Pay with cash on delivery.");

    public static readonly PaymentMethod Unknown =
        new(PaymentMethodCode.Unknown, nameof(Unknown), "Unknown payment method.");

    static PaymentMethod()
    {
        _ = CreditCard;
        _ = DebitCard;
        _ = PayPal;
        _ = BankTransfer;
        _ = CashOnDelivery;
        _ = Unknown;
    }

    public static Fin<PaymentMethod> FromCode(string code) =>
        Enum.TryParse<PaymentMethodCode>(code, true, out var statusCode)
            ? Optional(_all.FirstOrDefault(s => s.Code == statusCode))
                .ToFin(InvalidOperationError.New($"Invalid payment method code '{code}'"))
            : FinFail<PaymentMethod>(InvalidOperationError.New($"Invalid payment method code '{code}'"));

    public static Fin<PaymentMethod> FromName(string name) =>
        Optional(_all.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            .ToFin(InvalidOperationError.New($"Invalid payment method name '{name}'"));

    public static PaymentMethod FromUnsafe(string repr) =>
        _all.FirstOrDefault(s => s.Name.Equals(repr, StringComparison.OrdinalIgnoreCase)) ?? Unknown;
}
