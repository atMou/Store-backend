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
}

