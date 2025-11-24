using System.Numerics;

using Product.Domain.Enums;

namespace Product.Domain.ValueObjects;

public record Rating : IComparisonOperators<Rating, Rating, bool>
{
    private Rating() { }


    public RatingCode Code { get; }
    public double Value { get; }
    public string Description { get; }

    private static readonly List<Rating> _all = new();
    public static IReadOnlyList<Rating> All => _all;

    private Rating(RatingCode code, double value, string description)
    {
        Code = code;
        Value = value;
        Description = description;
        _all.Add(this);
    }
    public static Rating None => new(RatingCode.None, 0, "No Rating");
    public static Rating Poor => new(RatingCode.One, 1, "Poor");
    public static Rating Fair => new(RatingCode.Two, 2, "Fair");
    public static Rating Good => new(RatingCode.Three, 3, "Good");
    public static Rating VeryGood => new(RatingCode.Four, 4, "Very Good");
    public static Rating Excellent => new(RatingCode.Five, 5, "Excellent");


    public static Fin<Rating> From(double value) =>
        value switch
        {
            0 => None,
            < 2 => Poor,
            < 3 => Fair,
            < 4 => Good,
            < 5 => VeryGood,
            5 => Excellent,
            _ => FinFail<Rating>((Error)$"Invalid rating value '{value}'")
        };

    public static Rating FromUnsafe(double value) =>
        Optional(_all.FirstOrDefault(r => Math.Abs(r.Value - value) < 2))
            .Match(r => r, () => None);


    public virtual bool Equals(Rating? other)

    {
        return other is { } o &&
               GetType() == o.GetType() && Math.Abs(Value - o.Value) < 2;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }

    public static bool operator >(Rating left, Rating right)
    {
        return left.Value > right.Value;
    }

    public static bool operator >=(Rating left, Rating right)
    {
        return left.Value >= right.Value;
    }

    public static bool operator <(Rating left, Rating right)
    {
        return left.Value < right.Value;
    }

    public static bool operator <=(Rating left, Rating right)
    {
        return left.Value <= right.Value;
    }
}
