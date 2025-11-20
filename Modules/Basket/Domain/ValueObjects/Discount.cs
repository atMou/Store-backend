using Basket.Domain.Enums;

namespace Basket.Domain.ValueObjects;

public record Discount : DomainType<Discount, (Enums.DiscountType DiscountType, decimal DiscountValue)>
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
            //(DiscountType.None, _)
            //    => new Discount(DiscountType.None, 0),

            (DiscountType.Percentage, var v) when v is > 0 and <= 1
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
}