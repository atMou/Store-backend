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
    public CartId? CartId { get; private set; }
    public string Code { get; private init; }
    public Discount Discount { get; private set; }
    public Description Description { get; private set; }
    public ExpiryDate ExpiryDate { get; private set; }
    public decimal MinimumPurchaseAmount { get; private set; }
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
                Description.From(10, 200, description),
                ExpiryDate.From(expiryDate, currentDate),
                Discount.From((discountType, discountValue)))
            .Apply((desc, exp, discount) =>
                new Coupon(GenerateCode(), desc, exp, discount, minimumPurchaseAmount)).As();
    }

    public Fin<Coupon> MarkAsRedeemed(DateTime utcNow)
    {
        return EnsureIsValid(utcNow).Bind(_ =>
            CouponStatus.EnsureCanTransitionTo(CouponStatus.Redeemed)
            .Map(__ =>
            {
                CouponStatus = CouponStatus.Redeemed;
                return this;
            }));
    }

    public Fin<Coupon> ApplyToCart(CartId cartId, UserId userId, DateTime utcNow)
    {
        return (EnsureHasNoUser(), EnsureHasNoCart(), ExpiryDate.EnsureIsValid(utcNow))
            .Apply((_, _, _) =>
            {
                CartId = cartId;
                UserId = userId;
                CouponStatus = CouponStatus.AppliedToCart;
                return this;
            })
            .As();

    }

    public Fin<Coupon> AssignToUser(UserId userId, DateTime utcNow)
    {
        return (CouponStatus.EnsureCanTransitionTo(CouponStatus.AssignedToUser),
                ExpiryDate.EnsureIsValid(utcNow), EnsureHasNoUser())
                .Apply((_, _, _) =>
                {
                    UserId = userId;
                    CouponStatus = CouponStatus.AssignedToUser;
                    return this;
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

    public Coupon MarkAsDeleted()
    {
        IsDeleted = true;
        ExpiryDate = ExpiryDate.FromUnsafe(DateTime.UtcNow);
        return this;
    }


    public Fin<Coupon> MarkAsExpired(DateTime utcNow)
    {
        if (CouponStatus == CouponStatus.Expired)
        {
            return FinFail<Coupon>(NotFoundError.New($"Coupon with ID '{Id.Value}' is already expired."));
        }

        CouponStatus = CouponStatus.Expired;
        ExpiryDate = ExpiryDate.FromUnsafe(utcNow);
        return this;
    }

    public Fin<Coupon> ChangeCouponStatus(CouponStatus status)
    {
        return CouponStatus.EnsureCanTransitionTo(status).Map(_ =>
        {
            CouponStatus = status;
            return this;
        });
    }


    public Fin<Coupon> EnsureCanDelete()
    {
        if (IsDeleted) return FinFail<Coupon>(InvalidOperationError.New("Coupon is already deleted."));
        return (EnsureHasNoCart(), EnsureHasNoUser()).Apply((_, _) => this).As();
    }

    private Fin<Coupon> UpdateDescription(string repr)
    {
        return Description.From(10, 200, repr).Map(d =>
        {
            Description = d;
            return this;
        });
    }
    private Fin<Coupon> UpdateExpiryDate(DateTime dateTime, DateTime utcNow)
    {
        return ExpiryDate.From(dateTime, utcNow).Map(e =>
        {
            ExpiryDate = e;
            return this;
        });
    }
    public Fin<Coupon> Update(UpdateCouponDto dto, DateTime utcNow)
    {
        var fin = FinSucc(this);
        if (dto.Description is not null)
            fin = fin.Bind(coupon => coupon.UpdateDescription(dto.Description));

        if (dto.ExpiryDate is not null)
            fin = fin.Bind(coupon => coupon.UpdateExpiryDate(dto.ExpiryDate.Value, utcNow));

        if (dto.Status is { } cs)
            fin = fin.Bind(coupon => coupon.ChangeCouponStatus(cs));

        return EnsureIsValid(utcNow).Bind(_ => fin);
    }

    private Fin<Unit> EnsureIsValid(DateTime datetime)
    {
        if (CouponStatus == CouponStatus.Expired)
            return FinFail<Unit>(InvalidOperationError.New("Coupon is expired."));
        if (CouponStatus == CouponStatus.Redeemed)
            return FinFail<Unit>(InvalidOperationError.New("Coupon is redeemed."));
        return ExpiryDate.EnsureIsValid(datetime);


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