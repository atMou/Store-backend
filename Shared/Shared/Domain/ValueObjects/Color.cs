using Product.Domain.Enums;

namespace Shared.Domain.ValueObjects;

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

        _ = Silver;
        _ = Maroon;
        _ = Rose;
        _ = Coral;
        _ = Gold;
        _ = Lime;
        _ = Olive;
        _ = Teal;
        _ = Mint;
        _ = SkyBlue;
        _ = Cyan;
        _ = Violet;
        _ = Lavender;
        _ = Magenta;
        _ = Beige;
        _ = Tan;
        _ = Cream;
        _ = Burgundy;
        _ = Indigo;
        _ = Turquoise;
        _ = Peach;
        _ = Khaki;
        _ = Charcoal;
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

    public static Color Silver => new(ColorCode.SV, "Silver", "#C0C0C0");
    public static Color Maroon => new(ColorCode.MR, "Maroon", "#800000");
    public static Color Rose => new(ColorCode.RS, "Rose", "#FF007F");
    public static Color Coral => new(ColorCode.CR, "Coral", "#FF7F50");
    public static Color Gold => new(ColorCode.GD, "Gold", "#FFD700");
    public static Color Lime => new(ColorCode.LM, "Lime", "#00FF00");
    public static Color Olive => new(ColorCode.OL, "Olive", "#808000");
    public static Color Teal => new(ColorCode.TL, "Teal", "#008080");
    public static Color Mint => new(ColorCode.MT, "Mint", "#98FF98");
    public static Color SkyBlue => new(ColorCode.SB, "Sky Blue", "#87CEEB");
    public static Color Cyan => new(ColorCode.CY, "Cyan", "#00FFFF");
    public static Color Violet => new(ColorCode.VT, "Violet", "#EE82EE");
    public static Color Lavender => new(ColorCode.LV, "Lavender", "#E6E6FA");
    public static Color Magenta => new(ColorCode.MG, "Magenta", "#FF00FF");
    public static Color Beige => new(ColorCode.BE, "Beige", "#F5F5DC");
    public static Color Tan => new(ColorCode.TN, "Tan", "#D2B48C");
    public static Color Cream => new(ColorCode.CM, "Cream", "#FFFDD0");
    public static Color Burgundy => new(ColorCode.BG, "Burgundy", "#800020");
    public static Color Indigo => new(ColorCode.IN, "Indigo", "#4B0082");
    public static Color Turquoise => new(ColorCode.TQ, "Turquoise", "#40E0D0");
    public static Color Peach => new(ColorCode.PC, "Peach", "#FFE5B4");
    public static Color Khaki => new(ColorCode.KH, "Khaki", "#F0E68C");
    public static Color Charcoal => new(ColorCode.CH, "Charcoal", "#36454F");


    public static Fin<Color> From(string name) =>
        Optional(_all.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            .ToFin(ValidationError.New($"Invalid color name '{name}'"));

    public static Color FromUnsafe(string repr) =>

        Optional(_all.FirstOrDefault(c => c.Name == repr)).Match(
            c => c, () => Color.None);

    public static Color FromCodeUnsafe(string repr) =>
        Optional(_all.FirstOrDefault(c => c.Code.ToString() == repr)).Match(
            c => c, () => Color.None);
    public static IEnumerable<Color> Like(IEnumerable<string> repr)
    {
        return _all.Where(color => repr.Any(s => color.Name.Contains(s)));
    }

    public static Fin<IEnumerable<Color>> From(IEnumerable<string> colors)
    {
        return colors.AsIterable().Traverse(c => From(c)).Map(it => it.AsEnumerable()).As();
    }

    public virtual bool Equals(Color? other)
    {
        return other is not null &&
               Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
