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

        Code = code.ToUpperInvariant();
        Name = name;
        ConversionRate = conversionRate;
        _all.Add(this);
    }


    // Predefined currencies - EUR is the primary currency for this application
    public static readonly Currency None = new("NONE", "No Currency Specified", 0);
    public static readonly Currency EUR = new("EUR", "Euro", 1.0); // EUR is base currency (1.0 conversion rate)

    // Other currencies are kept for reference but not actively used
    public static readonly Currency USD = new("USD", "US Dollar", 1.09); // Updated to EUR base
    public static readonly Currency GBP = new("GBP", "British Pound", 0.85); // Updated to EUR base
    public static readonly Currency JPY = new("JPY", "Japanese Yen", 161.0); // Updated to EUR base
    public static readonly Currency CAD = new("CAD", "Canadian Dollar", 1.49); // Updated to EUR base
    public static readonly Currency AUD = new("AUD", "Australian Dollar", 1.71); // Updated to EUR base
    public static readonly Currency CHF = new("CHF", "Swiss Franc", 0.94); // Updated to EUR base

    /// <summary>
    /// Default currency for the application (EUR)
    /// </summary>
    public static readonly Currency Default = EUR;

    public bool IsNone => this == None;

    public bool IsDefault => this == Default;

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
        return _all.FirstOrDefault(c => c.Code == code.ToUpperInvariant()) ?? Default;
    }

    /// <summary>
    /// Get the default currency (EUR) for the application
    /// </summary>
    public static Currency GetDefault() => Default;
}