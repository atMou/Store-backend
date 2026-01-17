namespace Shared.Domain.ValueObjects;

public record Sku
{

    private Sku()
    {
    }

    private Sku(string category, string color, string brand, string size, string code)
    {
        Value = $"{category}-{brand}-{color}-{size}-{code}";
    }

    public string Value { get; private init; }


    public static Sku From(string category, string color, string size, string brand)
    {
        return new Sku(category, color, brand, size, Helpers.GenerateCode(6));
    }

    public static Sku FromUnsafe(string sku)
    {
        return new Sku { Value = sku };
    }

    public virtual bool Equals(Sku? other)
    {
        return other is not null &&
               Value == other.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }
}

