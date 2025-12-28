using System.Numerics;

namespace Product.Domain.ValueObjects;

public record Rating : IComparisonOperators<Rating, Rating, bool>
{
    private static readonly List<Rating> _all = new();

    private Rating()
    {
    }

    private Rating(double value, string description)
    {
        Value = value;
        Description = description;
        _all.Add(this);
    }


    public double Value { get; }
    public string Description { get; }
    public static Rating None => new(0, "No Rating");
    public static Rating Poor => new(1, "Poor");
    public static Rating Fair => new(2, "Fair");
    public static Rating Good => new(3, "Good");
    public static Rating VeryGood => new(4, "Very Good");
    public static Rating Excellent => new(5, "Excellent");

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


    public virtual bool Equals(Rating? other)

    {
        return other is { } o &&
               GetType() == o.GetType() && Math.Abs(Value - o.Value) < 2;
    }


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

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }
}