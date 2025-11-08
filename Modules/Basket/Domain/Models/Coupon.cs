using System.Text.RegularExpressions;

using Basket.Basket.Domain.ValueObjects;

using Shared.Domain.Abstractions;
using Shared.Domain.Validations;

namespace Basket.Domain.Models;

public record Coupon : Aggregate<CouponId>
{

    private Coupon(string code, string description, DateTime expiryDate, Discount discount) : base(CouponId.New)
    {
        Code = code;
        Description = description;
        ExpiryDate = expiryDate;
        Discount = discount;
    }

    public string Code { get; private set; }
    public UserId? UserId { get; private set; } = null;
    public CartId? AppliedOnCartId { get; private set; } = null;
    public Discount Discount { get; private set; }
    public string Description { get; private set; }
    private DateTime ExpiryDate { get; init; }
    public bool IsValid(DateTime dateTime) => ExpiryDate > dateTime;


    public static Fin<Coupon> Create(string code, string description, Enums.DiscountType discountType, decimal discountValue, DateTime expiryDate, DateTime dateTime) =>
        (ValidateCode(code),
            ValidateDescription(description),
            ValidateExpiryDate(expiryDate, dateTime),
            ValidateDiscount(discountType, discountValue))
            .Apply((_, _, _, discount) => new Coupon(code, description, expiryDate, discount)).As();

    private static Fin<Discount> ValidateDiscount(Enums.DiscountType discountType, decimal value)
    {
        return Discount.From((discountType, value));
    }

    private static Fin<Unit> ValidateExpiryDate(DateTime expiryDate, DateTime dateTime)
    {
        return expiryDate < dateTime.Date
            ? FinFail<Unit>(Error.New($"Coupon expiry date '{expiryDate:yyyy-MM-dd}' cannot be in the past."))
            : unit;
    }

    private static Fin<Unit> ValidateDescription(string description)
    {
        var length = description.Length;

        return (Helpers.MinLength10(description, $"Coupon {nameof(Description)}"),
              Helpers.MaxLength200(description, $"Coupon {nameof(Description)}"))
            .Apply((_, _) => unit).As().ToFin();
    }


    private static Fin<Unit> ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return FinFail<Unit>(Error.New("Coupon code cannot be empty."));

        var regex = new Regex(@"^[A-Za-z0-9]{5}(-[A-Za-z0-9]{5}){4}$", RegexOptions.Compiled);

        return regex.IsMatch(code)
            ? unit
            : FinFail<Unit>(Error.New("Invalid coupon code format. Expected: 5 groups of 5 alphanumeric characters separated by hyphens."));
    }


}