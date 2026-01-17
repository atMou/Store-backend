namespace Shared.Domain.ValueObjects;

public record PaymentId : IId
{
    private PaymentId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static PaymentId From(Guid value) => new(value);
    public static PaymentId New => new(Guid.NewGuid());

    public virtual bool Equals(PaymentId? other)
    {
        return other is not null && Value.Equals(other.Value);
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
