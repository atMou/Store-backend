using Shared.Domain.Enums;

namespace Shared.Domain.ValueObjects;

public record Size
{
    private static readonly List<Size> _all = new();

    static Size()
    {
        _ = None;
        _ = XS;
        _ = XXS;
        _ = Small;
        _ = Medium;
        _ = Large;
        _ = ExtraLarge;
        _ = XXL;
        _ = XXXL;
    }

    private Size()
    {
    }

    private Size(SizeCode code, string name, int order)
    {
        Code = code;
        Name = name;
        Order = order;
        _all.Add(this);
    }

    public SizeCode Code { get; }
    public string Name { get; }
    public int Order { get; }
    public static IReadOnlyList<Size> All => _all;

    // Define sizes
    public static Size None => new(SizeCode.None, "None", 0);

    public static Size XS => new(SizeCode.XS, "Extra Small", 2);
    public static Size XXS => new(SizeCode.XXS, "2X Small", 1);

    public static Size Small => new(SizeCode.S, "Small", 3);
    public static Size Medium => new(SizeCode.M, "Medium", 4);
    public static Size Large => new(SizeCode.L, "Large", 5);
    public static Size ExtraLarge => new(SizeCode.XL, "Extra Large", 6);
    public static Size XXL => new(SizeCode.XXL, "2X Large", 7);
    public static Size XXXL => new(SizeCode.XXXL, "3X Large", 8);

    public virtual bool Equals(Size? other)
    {
        return other is not null && Name == other.Name;
    }
    public Size Clones() => new Size(Code, Name, Order);

    public static Fin<Size> From(string code) =>
        Optional(_all.FirstOrDefault(s => s.Code.ToString().Equals(code, StringComparison.OrdinalIgnoreCase)))
            .ToFin(ValidationError.New($"Invalid size code '{code}'"));

    public static Fin<IEnumerable<Size>> From(IEnumerable<string> sizes)
    {
        return sizes.AsIterable().Traverse(size => From(size)).Map(it => it.AsEnumerable()).As();
    }

    public static Size FromUnsafe(string repr) =>
        Optional(_all.FirstOrDefault(s => s.Name == repr)).IfNone(() => None);

    public static Size FromCodeUnsafe(string repr) =>
       Optional(_all.FirstOrDefault(s => s.Code.ToString().Equals(repr, StringComparison.OrdinalIgnoreCase)))
           .IfNone(() => None);
    public static IEnumerable<Size> Like(IEnumerable<string> repr)
    {
        return _all.Where(size => repr.Any(s => size.Code.ToString().Contains(s)));
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }


}