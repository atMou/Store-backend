using Basket.Domain.Enums;

namespace Basket.Domain.Models;

public record Coupon : Aggregate<CouponId>
{
    private Coupon() : base(CouponId.New)
    {
    }

    private Coupon(
        string code,
        Description description,
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
    public Description Description { get; private init; }
    public ExpiryDate ExpiryDate { get; private set; }
    public decimal MinimumPurchaseAmount { get; private init; }
    public CouponStatus CouponStatus { get; private set; } = CouponStatus.Unknown;
    public bool IsDeleted { get; private set; }

    public static Fin<Coupon> Create(
        string description,
        decimal discountValue,
        DateTime expiryDate,
        DiscountType discountType,
        DateTime currentDate,
        decimal minimumPurchaseAmount
    )
    {
        return (
                Description.From(description),
                ExpiryDate.From(expiryDate, currentDate),
                Discount.From((discountType, discountValue)))
            .Apply((desc, exp, discount) =>
                new Coupon(GenerateCode(), desc, exp, discount, minimumPurchaseAmount)).As();
    }



    public Fin<Coupon> ApplyToCart(CartId cartId, UserId userId, DateTime utcNow)
    {
        return (EnsureHasNoUser(), EnsureHasNoCart(), ExpiryDate.EnsureIsValid(utcNow))
            .Apply((_, _, _) => this with { CartId = cartId, CouponStatus = CouponStatus.AppliedToCart })
            .As();

    }

    public Fin<Coupon> AssignToUser(UserId userId, DateTime utcNow)
    {
        return (CouponStatus.EnsureCanTransitionTo(CouponStatus.AssignedToUser),
                ExpiryDate.EnsureIsValid(utcNow), EnsureHasNoUser())
                .Apply((_, _, _) => this with
                {
                    UserId = userId,
                    CouponStatus = CouponStatus.AssignedToUser
                }).As();
    }

    private Fin<Unit> EnsureHasNoUser()
    {
        return UserId.IsNull() ? unit
            : FinFail<Unit>(InvalidOperationError.New("Coupon is already assigned to a user."));
    }
    private Fin<Unit> EnsureHasAUser()
    {
        return UserId.IsNull()
            ? FinFail<Unit>(InvalidOperationError.New("Coupon is already assigned to a user.")) :
            unit;
    }

    private Fin<Unit> EnsureHasNoCart()
    {
        return CartId.IsNull() ? unit
    : FinFail<Unit>(InvalidOperationError.New("Coupon is already assigned to a cart."));
    }
    private Fin<Unit> EnsureHasACart()
    {
        return CartId.IsNull()
            ? FinFail<Unit>(InvalidOperationError.New("Coupon is already assigned to a cart."))
            : unit;
    }
    public Fin<decimal> GetDiscountFromTotal(decimal total, DateTime utcNow)
    {
        return (EnsureHasACart(), EnsureHasAUser(), ExpiryDate.EnsureIsValid(utcNow))
               .Apply((_, _, _) => Discount.Apply(total)).As();
    }

    public Coupon MarkAsDeleted()
    {
        return this with { IsDeleted = true, ExpiryDate = ExpiryDate.FromUnsafe(DateTime.UtcNow) };
    }


    public Fin<Coupon> MarkAsExpired(DateTime utcNow)
    {
        if (CouponStatus == CouponStatus.Expired)
        {
            return FinFail<Coupon>(NotFoundError.New($"Coupon with ID '{Id.Value}' is already expired."));
        }
        return this with { CouponStatus = CouponStatus.Expired, ExpiryDate = ExpiryDate.FromUnsafe(utcNow) };
    }


    // should be called from outside when redeeming the coupon or when user add it to cart
    public Fin<Coupon> ChangeCouponStatus(CouponStatus status)
    {
        return CouponStatus.EnsureCanTransitionTo(status).Map(_ => this with { CouponStatus = status });
    }


    public Fin<Coupon> EnsureCanDelete()
    {
        if (IsDeleted) return FinFail<Coupon>(InvalidOperationError.New("Coupon is already deleted."));
        return (EnsureHasNoCart(), EnsureHasNoUser()).Apply((_, _) => this).As();
    }

    public Fin<Coupon> Update(UpdateCouponDto dto, DateTime utcNow)
    {
        var validations = Seq<Fin<Coupon>>();
        if (dto.Description is not null)
            validations = validations.Add(
                  Description.From(dto.Description).Map(d =>
                      this with { Description = d }));
        if (dto.ExpiryDate is not null)
            validations = validations.Add(
                Optional(dto.ExpiryDate).ToFin().Bind(ex => ExpiryDate.From(ex, utcNow).Map(e =>
                    this with { ExpiryDate = e })));
        if (dto.DiscountType is not null)
            validations = validations.Add(
                (Optional(dto.DiscountValue).ToFin(InvalidOperationError.New("Invalid discount value.")),
                    Optional(dto.DiscountType).ToFin(InvalidOperationError.New("Invalid discount type.")))
                .Apply((d, type) => (type, d))
                .Bind(t => Discount.From((t.type, t.d)).Map(discount =>
                    this with { Discount = discount })).As());

        if (dto.Status is { } cs)
            validations = validations.Add(CouponStatus.EnsureCanTransitionTo(cs).Map(_ => this with { CouponStatus = cs }));

        if (dto.MinimumPurchaseAmount is not null)
            validations = validations.Add(
                ValidateMinimumPurchaseAmount(dto.MinimumPurchaseAmount).Map(_ =>
                    this with { MinimumPurchaseAmount = dto.MinimumPurchaseAmount.Value }));

        return EnsureIsValid(utcNow).Bind(_ => validations
            .Traverse(identity)
            .Map(seq => seq.Last.Match(coupon => coupon, () => this))
        );
    }

    private Fin<Unit> EnsureIsValid(DateTime datetime)
    {
        if (CouponStatus == CouponStatus.Expired)
            return FinFail<Unit>(InvalidOperationError.New("Coupon is expired."));
        if (CouponStatus == CouponStatus.Redeemed)
            return FinFail<Unit>(InvalidOperationError.New("Coupon is redeemed."));
        return ExpiryDate.EnsureIsValid(datetime);


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