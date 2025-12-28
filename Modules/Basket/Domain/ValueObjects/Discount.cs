using System.Numerics;

using Basket.Domain.Enums;

namespace Basket.Domain.ValueObjects;

public record Discount :
    DomainType<Discount, (DiscountType DiscountType, decimal DiscountValue)>,
    IComparable<Discount>,
    IComparisonOperators<Discount, Discount, bool>,
    IAdditionOperators<Discount, Discount, Discount>,
    ISubtractionOperators<Discount, Discount, Discount>,
    IAdditiveIdentity<Discount, Discount>



{
    public DiscountType DiscountType { get; }
    public decimal DiscountValue { get; }

    private Discount(DiscountType discountType, decimal discountValue)
    {
        DiscountType = discountType;
        DiscountValue = discountValue;
    }
    public static Fin<Discount> From((DiscountType DiscountType, decimal DiscountValue) repr)
    {
        return repr switch
        {


            (DiscountType.Percentage, var v) when v is > 0 and <= 100
                => new Discount(DiscountType.Percentage, v),

            (DiscountType.Amount, var v) when v > 0
                => new Discount(DiscountType.Amount, v),

            _ => FinFail<Discount>(BadRequestError.New(
                $"Invalid discount: type '{repr.DiscountType.ToString()}', value '{repr.DiscountValue}'"
            ))
        };
    }

    public (DiscountType DiscountType, decimal DiscountValue) To() => (DiscountType, DiscountValue);

    public decimal Apply(decimal total) =>
        DiscountType switch
        {
            //DiscountType.None => total,
            DiscountType.Percentage => total - (total * (1 - DiscountValue)),
            DiscountType.Amount => Math.Max(0, DiscountValue),
            _ => total
        };

    public int CompareTo(Discount? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        return (this, other) switch
        {
            (_, null) => 1,
            var (f, s) when f.DiscountType == s.DiscountType => f.DiscountValue.CompareTo(s.DiscountValue),
            _ => throw new InvalidOperationException("Invalid comparison operation for discount")
        };
    }


    public static bool operator >(Discount left, Discount right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Discount left, Discount right)
    {
        return left.CompareTo(right) >= 0;
    }

    public static bool operator <(Discount left, Discount right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Discount left, Discount right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static Discount operator +(Discount left, Discount right)
    {
        return (left, right) switch
        {
            var (l, r) when l.DiscountType == r.DiscountType =>
                new Discount(l.DiscountType, l.DiscountValue + r.DiscountValue),
            _ => throw new InvalidOperationException("Invalid addition operation for discount")
        };
    }

    public static Discount AdditiveIdentity => new Discount(DiscountType.Amount, 0M);
    public static Discount operator -(Discount left, Discount right)
    {
        return (left, right) switch
        {
            var (l, r) when l.DiscountType == r.DiscountType =>
                new Discount(l.DiscountType, Math.Max(0, l.DiscountValue - r.DiscountValue)),
            _ => throw new InvalidOperationException("Invalid subtraction operation for discount")
        };
    }
}