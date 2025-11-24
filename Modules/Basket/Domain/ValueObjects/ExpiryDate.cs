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
            return FinFail<ExpiryDate>(InvalidOperationError.New("Expiry date cannot be in the past."));

        return new ExpiryDate(repr);
    }

    public DateTime To() => Value;

    public static ExpiryDate FromUnsafe(DateTime repr)
    {
        return new ExpiryDate(repr);
    }

    public Fin<Unit> EnsureIsValid(DateTime utcNow)
    {
        return Value > utcNow ? unit : FinFail<Unit>(InvalidOperationError.New($"Expired at '{Value:yyyy-MM-dd}'"));
    }




}