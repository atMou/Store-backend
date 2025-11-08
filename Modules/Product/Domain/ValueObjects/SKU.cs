namespace Product.Domain.ValueObjects;

public record Sku
{
    private static readonly Regex _skuRegex =
        new("^[A-Z0-9]+$", RegexOptions.Compiled);

    public readonly string Value;

    private Sku()
    {
    }

    private Sku(string category, string color, string size, string brand, string code)
    {
        Value = $"{category}-{color}-{size}-{brand}-{code}";
    }

    public static Sku From(string category, string color, string size, string brand)
    {
        return new Sku(category, color, size, brand, GenerateSkuCode());
    }

    private static string GenerateSkuCode()
    {
        return Range('A', 'Z').AsIterable().Concat(Range('0', '9').AsIterable()).Take(6).ToString();
    }
}

//[Category]-[Color]-[Size]-[Brand]-[UniqueID]