namespace Shared.Domain.ValueObjects;

public record Sku
{
    private static readonly Regex _skuRegex =
        new("^[A-Z0-9]+$", RegexOptions.Compiled);

    private Sku()
    {
    }

    private Sku(string category, string color, string size, string brand, string code)
    {
        Value = $"{category}-{color}-{size}-{brand}-{code}";
    }

    public string Value { get; private set; }

    public static Sku From(string category, string color, string size, string brand)
    {
        return new Sku(category, color, size, brand, GenerateSkuCode());
    }

    public static Sku FromUnsafe(string sku)
    {
        return new Sku { Value = sku };
    }

    private static string GenerateSkuCode()
    {
        var iterable = Range('A', 'Z').Concat(Range('0', '9')).ToArray();
        var random = new Random();
        random.Shuffle(iterable);
        return new string(iterable.Take(6).ToArray());
    }
}

//[Category]-[Color]-[Size]-[Brand]-[UniqueID]