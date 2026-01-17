using System.Numerics;

using Basket.Domain.Enums;

namespace Basket.Domain.ValueObjects;

public record Discount :
    IComparable<Discount>,
    IComparisonOperators<Discount, Discount, bool>,
    IAdditionOperators<Discount, Discount, Discount>,
    ISubtractionOperators<Discount, Discount, Discount>,
    IAdditiveIdentity<Discount, Discount>
{
    public DiscountType DiscountType { get; }
    public decimal DiscountValue { get; }

    public static Discount Zero => new(DiscountType.Amount, 0);
    private Discount(DiscountType discountType, decimal discountValue)
    {
        DiscountType = discountType;
        DiscountValue = discountValue;
    }

    public static Fin<Discount> From(string discountTypeString, decimal discountValue)
    {
        if (!Enum.TryParse<DiscountType>(discountTypeString, ignoreCase: true, out var discountType))
        {
            return FinFail<Discount>(BadRequestError.New(
                $"Invalid discount type: '{discountTypeString}'. Valid types are: {string.Join(", ", Enum.GetNames<DiscountType>())}"
            ));
        }

        return (discountType, discountValue) switch
        {
            (DiscountType.Percentage, var v) when v is > 0 and <= 100
                => new Discount(DiscountType.Percentage, v),

            (DiscountType.Amount, var v) when v > 0
                => new Discount(DiscountType.Amount, v),

            _ => FinFail<Discount>(BadRequestError.New(
                $"Invalid discount: type '{discountType}', value '{discountValue}'"
            ))
        };
    }

    public decimal Apply(Money total) =>
        DiscountType switch
        {
            DiscountType.Percentage => total.Value * (DiscountValue / 100m),
            DiscountType.Amount => Math.Min(DiscountValue, total.Value),
            _ => 0m
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