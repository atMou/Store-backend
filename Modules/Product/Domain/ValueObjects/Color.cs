using Product.Domain.Enums;

namespace Product.Domain.ValueObjects;

using static LanguageExt.Prelude;

public record Color
{
    private Color() { }

    public ColorCode Code { get; }
    public string Name { get; }
    public string Hex { get; } // Optional: store HEX code

    private static readonly List<Color> _all = new();
    public static IReadOnlyList<Color> All => _all;

    private Color(ColorCode code, string name, string hex)
    {
        Code = code;
        Name = name;
        Hex = hex;
        _all.Add(this);
    }
    static Color()
    {
        _ = None;
        _ = Red;
        _ = Blue;
        _ = Green;
        _ = Black;
        _ = White;
        _ = Yellow;
        _ = Orange;
        _ = Purple;
        _ = Pink;
        _ = Brown;
        _ = Gray;
        _ = Navy;
    }

    public static Color None => new(ColorCode.None, "", "");
    public static Color Red => new(ColorCode.RD, "Red", "#FF0000");
    public static Color Blue => new(ColorCode.BL, "Blue", "#0000FF");
    public static Color Green => new(ColorCode.GR, "Green", "#008000");
    public static Color Black => new(ColorCode.BK, "Black", "#000000");
    public static Color White => new(ColorCode.WH, "White", "#FFFFFF");
    public static Color Yellow => new(ColorCode.YL, "Yellow", "#FFFF00");
    public static Color Orange => new(ColorCode.OR, "Orange", "#FFA500");
    public static Color Purple => new(ColorCode.PR, "Purple", "#800080");
    public static Color Pink => new(ColorCode.PK, "Pink", "#FFC0CB");
    public static Color Brown => new(ColorCode.BR, "Brown", "#8B4513");
    public static Color Gray => new(ColorCode.GY, "Gray", "#808080");
    public static Color Navy => new(ColorCode.NV, "Navy", "#000080");

    // Lookups
    public static Fin<Color> FromCode(string code) =>
        Enum.TryParse<ColorCode>(code, out var colorCode)
            ? Optional(_all.FirstOrDefault(c => c.Code == colorCode))
                .ToFin((Error)$"Invalid color code '{code}'")
            : FinFail<Color>((Error)$"Invalid color code '{code}'");

    public static Fin<Color> FromName(string name) =>
        Optional(_all.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            .ToFin((Error)$"Invalid color name '{name}'");

    public static Color FromUnsafe(string code) =>
        Enum.TryParse<ColorCode>(code, out var colorCode)
            ? Optional(_all.FirstOrDefault(c => c.Code == colorCode)).Match(
                c => c, () => Color.None)
            : None;

    public static Fin<IEnumerable<Color>> FromCodes(IEnumerable<string> colors)
    {
        return colors.AsIterable().Traverse(c => FromName(c)).Map(it => it.AsEnumerable()).As();
    }
}
