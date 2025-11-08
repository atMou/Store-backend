namespace Shared.Domain.ValueObjects;

public sealed record Currency
{
    private static readonly List<Currency> _all = new();
    public string Code { get; }
    public string Name { get; }
    public double ConversionRate { get; }
    public static IReadOnlyList<Currency> All => _all;

    private Currency()
    {
    }
    private Currency(string code, string name, double conversionRate)
    {

        Code = code.ToUpperInvariant(); ;
        Name = name;
        ConversionRate = conversionRate;
        _all.Add(this);
    }


    // Predefined currencies
    public static readonly Currency None = new("NONE", "No Currency Specified", 0);
    public static readonly Currency USD = new("USD", "US Dollar", 1.0);
    public static readonly Currency EUR = new("EUR", "Euro", 0.92);
    public static readonly Currency GBP = new("GBP", "British Pound", 0.78);
    public static readonly Currency JPY = new("JPY", "Japanese Yen", 110.25);
    public static readonly Currency CAD = new("CAD", "Canadian Dollar", 1.36);
    public static readonly Currency AUD = new("AUD", "Australian Dollar", 1.48);
    public static readonly Currency CHF = new("CHF", "Swiss Franc", 0.89);




    public bool IsNone => this == None;

    public override string ToString()
    {
        return Code;
    }

    public static Fin<Currency> FromCode(string code)
    {
        return Optional(_all.FirstOrDefault(c => string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase)))
            .ToFin(Error.New($"Currency code '{code}' is invalid."));
    }

    public static Fin<Currency> FromName(string name)
    {
        return Optional(_all.FirstOrDefault(c =>
                string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
            .ToFin(Error.New($"Currency name '{name}' is invalid."));
    }

    public static Currency FromUnsafe(string code)
    {
        return _all.FirstOrDefault(c => c.Code == code.ToUpperInvariant()) ?? None; ;
    }
}