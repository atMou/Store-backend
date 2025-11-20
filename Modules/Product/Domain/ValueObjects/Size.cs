using Product.Domain.Enums;

namespace Product.Domain.ValueObjects;

public record Size
{
    private Size() { }

    public SizeCode Code { get; }
    public string Name { get; }
    public int Order { get; } // useful for sorting sizes (XS < S < M < L...)

    private static readonly List<Size> _all = new();
    public static IReadOnlyList<Size> All => _all;

    private Size(SizeCode code, string name, int order)
    {
        Code = code;
        Name = name;
        Order = order;
        _all.Add(this);
    }
    static Size()
    {
        _ = None;
        _ = ExtraSmall;
        _ = Small;
        _ = Medium;
        _ = Large;
        _ = ExtraLarge;
        _ = XXL;
        _ = XXXL;
    }

    // Define sizes
    public static Size None => new(SizeCode.None, "None", 0);
    public static Size ExtraSmall => new(SizeCode.XS, "Extra Small", 1);
    public static Size Small => new(SizeCode.S, "Small", 2);
    public static Size Medium => new(SizeCode.M, "Medium", 3);
    public static Size Large => new(SizeCode.L, "Large", 4);
    public static Size ExtraLarge => new(SizeCode.XL, "Extra Large", 5);
    public static Size XXL => new(SizeCode.XXL, "2X Large", 6);
    public static Size XXXL => new(SizeCode.XXXL, "3X Large", 7);

    public static Fin<Size> FromCode(string code) =>
        Optional(_all.FirstOrDefault(s => s.Code.ToString() == code))
            .ToFin((Error)$"Invalid size code '{code}'");

    public static Fin<Size> FromName(string name) =>
        Optional(_all.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            .ToFin((Error)$"Invalid size name '{name}'");


    public static Size FromUnsafe(string code) =>
        Optional(_all.FirstOrDefault(s => s.Code.ToString() == code)).IfNone(() => None);
    public static IReadOnlyList<string> Sizes() =>
        _all.OrderBy(s => s.Order).Select(s => s.Code.ToString()).ToList();
}
