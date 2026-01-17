namespace Basket.Domain.Models;

public class Coupon : Aggregate<CouponId>
{
    private Coupon() : base(CouponId.New)
    {
    }

    private Coupon(
        string code,
        Description description,
        ExpiryDate expiryDate,
        Discount discount,
        decimal minimumPurchaseAmount) : base(CouponId.New)
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
        string discountType,
        decimal minimumPurchaseAmount,
        DateTime currentDate
    )
    {
        return (
                from desc in Description.From(10, 200, description)
                from exp in ExpiryDate.From(expiryDate, currentDate)
                from discount in Discount.From(discountType, discountValue)
                select new Coupon(GenerateCode(), desc, exp, discount, minimumPurchaseAmount)
            ).As();
    }
    public Coupon AddUserId(UserId userId)
    {
        UserId = userId;
        return this;
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
        return (
                from _ in EnsureHasNoUserOrItsSameUser(userId)
                from __ in CouponStatus.EnsureCanTransitionTo(CouponStatus.AppliedToCart)
                    .MapFail(_ => InvalidOperationError.New($"Coupon you trying to add is in use."))
                from ___ in EnsureHasNoCart()
                from c in ExpiryDate.EnsureIsValid(utcNow).Map(_ =>
                {
                    CartId = cartId;
                    UserId = userId;
                    CouponStatus = CouponStatus.AppliedToCart;
                    return this;
                })
                select c
            ).As();
    }

    public Fin<Coupon> RemoveFromCart()
    {
        return ChangeCouponStatus(CouponStatus.AssignedToUser).Map(coupon =>
        {
            CouponStatus = CouponStatus.AssignedToUser;
            CartId = null;
            return this;
        });
    }

    public Fin<Coupon> AssignToUser(UserId userId, DateTime utcNow)
    {
        return (
                from _ in CouponStatus.EnsureCanTransitionTo(CouponStatus.AssignedToUser)
                from __ in ExpiryDate.EnsureIsValid(utcNow)
                from c in EnsureHasNoUserOrItsSameUser(userId).Map(_ =>
                {
                    UserId = userId;
                    CouponStatus = CouponStatus.AssignedToUser;
                    return this;

                })
                select c
            ).As();
    }

    private Fin<Unit> EnsureHasNoUserOrItsSameUser(UserId userId)
    {
        return UserId.IsNull() || UserId == userId
            ? unit
            : FinFail<Unit>(InvalidOperationError.New("Coupon is already assigned to a user."));
    }

    private Fin<Unit> EnsureHasAUser()
    {
        return UserId.IsNull()
            ? FinFail<Unit>(InvalidOperationError.New("Coupon must be assigned to a user."))
            : unit;
    }

    private Fin<Unit> EnsureHasNoCart()
    {
        return CartId.IsNull()
            ? unit
            : FinFail<Unit>(InvalidOperationError.New("Coupon is already assigned to a cart."));
    }

    private Fin<Unit> EnsureHasACart()
    {
        return CartId.IsNull()
            ? FinFail<Unit>(InvalidOperationError.New("Coupon must be assigned to a cart."))
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

    public Fin<Coupon> EnsureCanDelete(UserId userId)
    {
        if (IsDeleted)
            return FinFail<Coupon>(InvalidOperationError.New("Coupon is already deleted."));

        return (
                from _ in EnsureHasNoCart()
                from __ in EnsureHasNoUserOrItsSameUser(userId)
                select this
            ).As();
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