namespace Shared.Domain.ValueObjects;

public record CouponId : IId
{
    private CouponId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static CouponId From(Guid value) => new(value);
    public static CouponId New => new(Guid.NewGuid());
    public virtual bool Equals(CouponId? other)
    {
        return other is not null && Value.Equals(other.Value);
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

