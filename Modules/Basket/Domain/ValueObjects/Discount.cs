using Basket.Enums;

namespace Basket.Basket.Domain.ValueObjects;

public record Discount : DomainType<Discount, (DiscountType DiscountType, decimal DiscountValue)>
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
            (DiscountType.None, _)
                => new Discount(DiscountType.None, 0),

            (DiscountType.Percentage, var v) when v is > 0 and <= 1
                => new Discount(DiscountType.Percentage, v),

            (DiscountType.Amount, var v) when v > 0
                => new Discount(DiscountType.Amount, v),

            _ => FinFail<Discount>(Error.New(
                $"Invalid discount: type={repr.DiscountType}, value={repr.DiscountValue}"
            ))
        };
    }

    public (DiscountType DiscountType, decimal DiscountValue) To() => (DiscountType, DiscountValue);

    public decimal Apply(decimal lineTotal) =>
        DiscountType switch
        {
            DiscountType.None => lineTotal,
            DiscountType.Percentage => lineTotal * (1 - DiscountValue),
            DiscountType.Amount => Math.Max(0, lineTotal - DiscountValue),
            _ => lineTotal
        };
}