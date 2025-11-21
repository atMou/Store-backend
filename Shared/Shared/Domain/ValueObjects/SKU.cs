using Shared.Domain.Validations;

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
        return new Sku(category, color, size, brand, Helpers.GenerateCode(6));
    }

    public static Sku FromUnsafe(string sku)
    {
        return new Sku { Value = sku };
    }


}

//[Category]-[Color]-[Size]-[Brand]-[UniqueID]