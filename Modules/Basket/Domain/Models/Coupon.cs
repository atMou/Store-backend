using Basket.Domain.Enums;

namespace Basket.Domain.Models;

public record Coupon : Aggregate<CouponId>
{
    private Coupon() : base(CouponId.New)
    {
    }

    private Coupon(
        string code,
        string description,
        ExpiryDate expiryDate,
        Discount discount, decimal minimumPurchaseAmount) : base(CouponId.New)
    {
        Code = code;
        Description = description;
        ExpiryDate = expiryDate;
        Discount = discount;
        MinimumPurchaseAmount = minimumPurchaseAmount;
    }

    public UserId? UserId { get; private set; }
    public CartId? CartId { get; private init; }
    public string Code { get; private init; }
    public Discount Discount { get; private set; }
    public string Description { get; private init; }
    public ExpiryDate ExpiryDate { get; private set; }
    public decimal MinimumPurchaseAmount { get; private init; }
    public CouponStatus Status { get; private set; } = CouponStatus.Unknown;
    public bool IsDeleted { get; private set; }

    public static Fin<Coupon> Create(
        string Description,
        decimal DiscountValue,
        DateTime expiryDate,
        DiscountType DiscountType,
        DateTime CurrentDate,
        decimal MinimumPurchaseAmount
    )
    {
        return (
                ValidateDescription(Description),
                ExpiryDate.From(expiryDate, CurrentDate),
                Discount.From((DiscountType, DiscountValue)))
            .Apply((_, _expiryDate, _discount) =>
                new Coupon(GenerateCode(), Description, _expiryDate, _discount, MinimumPurchaseAmount)).As();
    }



    public Fin<Coupon> ApplyToCart(CartId cartId, UserId userId, DateTime utcNow)
    {
        return from _ in CartId is not null
                ? FinFail<Unit>(InvalidOperationError.New("Coupon is already assigned to a cart."))
                : UserId is not null
                    ? FinFail<Unit>(InvalidOperationError.New("Coupon is already assigned to a user."))

                : Status switch
                {
                    { Value: 0 } => unit,
                    { Value: 1 } => unit,
                    { Value: 2 } => FinFail<Unit>(InvalidOperationError.New("Cannot assign an applied coupon.")),
                    { Value: 3 } => FinFail<Unit>(InvalidOperationError.New("Cannot assign an expired coupon.")),
                    { Value: 4 } => FinFail<Unit>(InvalidOperationError.New("Cannot assign a redeemed coupon.")),
                    _ => FinFail<Unit>(InvalidOperationError.New("Cannot assign coupon with unknown status."))
                }
               from _1 in ExpiryDate.IsValid(utcNow)
               select this with { CartId = cartId, Status = CouponStatus.AppliedToCart };
    }


    public Fin<decimal> GetDiscountFromTotal(decimal total, DateTime utcNow)
    {
        return from _1 in CartId.IsNull()
                ? FinFail<Unit>(InvalidOperationError.New("Coupon is not assigned to a cart."))
                : unit
               from _2 in UserId.IsNull()
                   ? FinFail<Unit>(InvalidOperationError.New("Coupon is not assigned to a user"))
                   : unit
               from _3 in ExpiryDate.IsValid(utcNow)
               select Discount.Apply(total);
    }

    public Coupon MarkAsDeleted()
    {
        return this with { IsDeleted = true, ExpiryDate = ExpiryDate.FromUnsafe(DateTime.UtcNow) };
    }


    public Coupon SetExpired(DateTime utcNow)
    {
        return this with { Status = CouponStatus.Expired, ExpiryDate = ExpiryDate.FromUnsafe(utcNow) };
    }


    private static Fin<Unit> ValidateExpiryDate(DateTime expiryDate, DateTime currentDate)
    {
        return expiryDate < currentDate.Date
            ? FinFail<Unit>(Error.New($"Coupon expiry date '{expiryDate:yyyy-MM-dd}' cannot be in the past."))
            : unit;
    }

    private static Fin<Unit> ValidateDescription(string description)
    {
        return (Helpers.MinLength10(description, $"Coupon {nameof(Description)}"),
                Helpers.MaxLength200(description, $"Coupon {nameof(Description)}"))
            .Apply((_, _) => unit).As().ToFin();
    }

    // should be called from outside when redeeming the coupon or when user add it to cart
    public Fin<Coupon> ChangeCouponStatus(CouponStatus status)
    {
        return Status.IsAllowedStatusChange(status).Map(_ => this with { Status = status });
    }

    // should be called when assigning coupon to a user
    public Fin<Coupon> SetUser(UserId userId)
    {
        return UserId is not null
            ? FinFail<Coupon>(InvalidOperationError.New("Coupon is already assigned to a user."))
            : ChangeCouponStatus(CouponStatus.AssignedToUser).Map(_ =>
            {
                UserId = userId;
                Status = CouponStatus.AssignedToUser;
                return this;
            });
    }

    //private static Fin<Unit> ValidateCode(string code)
    //{
    //    if (string.IsNullOrWhiteSpace(code))
    //        return FinFail<Unit>(Error.New("Coupon code cannot be empty."));

    //    var regex = new Regex(@"^[A-Za-z0-9]{5}(-[A-Za-z0-9]{5}){4}$", RegexOptions.Compiled);

    //    return regex.IsMatch(code)
    //        ? unit
    //        : FinFail<Unit>(Error.New("Invalid coupon code."));
    //}

    public Fin<Coupon> Update(UpdateCouponDto dto, DateTime utcNow)
    {
        List<Fin<Coupon>> validations = [];
        if (dto.Description is not null)
            validations.Add(
                ValidateDescription(dto.Description).Map(_ =>
                    this with { Description = dto.Description! }));
        if (dto.ExpiryDate is not null)
            validations.Add(
                Optional(dto.ExpiryDate).ToFin().Bind(ex => ExpiryDate.From(ex, utcNow).Map(e =>
                    this with { ExpiryDate = e })));
        if (dto.DiscountType is not null)
            validations.Add(
                (Optional(dto.DiscountValue).ToFin(InvalidOperationError.New("Invalid discount value.")),
                    Optional(dto.DiscountType).ToFin(InvalidOperationError.New("Invalid discount type.")))
                .Apply((d, type) => (type, d))
                .Bind(t => Discount.From((t.type, t.d)).Map(discount =>
                    this with { Discount = discount })).As());

        if (dto.Status is { } cs)
            validations.Add(Status.IsAllowedStatusChange(cs).Map(_ => this with { Status = cs }));

        if (dto.MinimumPurchaseAmount is not null)
            validations.Add(
                ValidateMinimumPurchaseAmount(dto.MinimumPurchaseAmount).Map(_ =>
                    this with { MinimumPurchaseAmount = dto.MinimumPurchaseAmount.Value }));

        return CanEdit().Bind(_ => validations.AsIterable()
            .Traverse(identity)
            .Map(_ => this)
        );
    }

    public Fin<Unit> CanEdit()
    {
        return Status switch
        {
            { Value: 0 } => unit,
            { Value: 1 } => unit,
            { Value: 2 } => FinFail<Unit>(InvalidOperationError.New("Cannot edit an applied coupon.")),
            { Value: 3 } => FinFail<Unit>(InvalidOperationError.New("Cannot edit an expired coupon.")),
            { Value: 4 } => FinFail<Unit>(InvalidOperationError.New("Cannot edit a redeemed coupon.")),
            _ => FinFail<Unit>(InvalidOperationError.New("Cannot edit coupon with unknown status."))
        };
    }

    private Fin<decimal?> ValidateMinimumPurchaseAmount(decimal? minimumAmount)
    {
        return minimumAmount is null ? FinSucc(minimumAmount)
            : minimumAmount < 0 ? FinFail<decimal?>(Error.New("Invalid minimum purchase amount."))
            : minimumAmount;
    }


    private static string GenerateCode()
    {
        var random = new Random();
        char[] arr = [.. "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"];
        return string.Join('-', Range(1, 5).Select(_ => ShuffleAndTake5(arr)));

        string ShuffleAndTake5(char[] ar)
        {
            random.Shuffle(ar);
            return new string(ar.Take(5).ToArray());
        }
    }
}