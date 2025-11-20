namespace Basket.Domain.ValueObjects;

public record ExpiryDate
{
    private ExpiryDate(DateTime value)
    {
        Value = value;
    }

    public DateTime Value { get; }

    public static Fin<ExpiryDate> From(DateTime repr, DateTime utcNow)
    {
        if (repr < utcNow)
            return FinFail<ExpiryDate>(Error.New("Expiry date cannot be in the past."));

        return new ExpiryDate(repr);
    }

    public DateTime To() => Value;

    public static ExpiryDate FromUnsafe(DateTime repr)
    {
        return new ExpiryDate(repr);
    }

    public Fin<Unit> IsValid(DateTime utcNow)
    {
        return Value > utcNow ? unit : FinFail<Unit>(Error.New("Expiry date cannot be in the past."));
    }
}