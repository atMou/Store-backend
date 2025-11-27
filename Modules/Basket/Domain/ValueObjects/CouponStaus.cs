namespace Basket.Domain.ValueObjects;
using LanguageExt;


public record CouponStatus
{
    private static readonly List<CouponStatus> _all = new();
    public static IReadOnlyList<CouponStatus> All => _all;

    public string Name { get; }
    public int Value { get; }
    public IEnumerable<CouponStatus> AllowedStatusChange { get; private set; }

    private CouponStatus(int value, string name)
    {
        Value = value;
        Name = name;

        _all.Add(this);
        AllowedStatusChange = [];
    }

    public static readonly CouponStatus Unknown = new(0, nameof(Unknown));
    public static readonly CouponStatus AssignedToUser = new(1, nameof(AssignedToUser));
    public static readonly CouponStatus AppliedToCart = new(2, nameof(AppliedToCart));
    public static readonly CouponStatus Expired = new(3, nameof(Expired));
    public static readonly CouponStatus Redeemed = new(4, nameof(Redeemed));

    static CouponStatus()
    {
        Unknown.AllowedStatusChange = [AssignedToUser, Expired];
        AssignedToUser.AllowedStatusChange = [AppliedToCart, AssignedToUser, Expired];
        AppliedToCart.AllowedStatusChange = [Redeemed];
        Expired.AllowedStatusChange = [];
        Redeemed.AllowedStatusChange = [];
    }

    public static Fin<CouponStatus> From(string name) =>
        Optional(_all.FirstOrDefault(s =>
                s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            .ToFin(InvalidOperationError.New($"Invalid coupon status '{name}'"));


    public static CouponStatus FromUnsafe(string name) =>
        From(name).IfFail(Unknown);

    public Fin<Unit> EnsureCanTransitionTo(CouponStatus status) =>
        AllowedStatusChange.Contains(status)
            ? unit
            : FinFail<Unit>(InvalidOperationError.New(
                $"Invalid coupon status change from {Name} to {status.Name}."));
}
