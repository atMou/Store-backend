using Product.Domain.Enums;

namespace Product.Domain.ValueObjects;

public record Category
{
    private static readonly List<Category> _all = [];
    private readonly IEnumerable<Category> _allowedSubcategories;

    private Category()
    {
    }

    private Category(CategoryCode code, string name, IEnumerable<Category> allowedSubcategories)
    {
        Code = code;
        Name = name;
        _allowedSubcategories = allowedSubcategories ?? [];
        _all.Add(this);
    }

    public CategoryCode Code { get; }
    public string Name { get; }
    public static IReadOnlyList<Category> All => _all;

    public static Category None => new(CategoryCode.None, "", []);

    // Top-level
    public static Category Men => new(CategoryCode.MN, nameof(Men), [
        MenTops, MenBottoms, MenActivewear, MenAccessories
    ]);

    public static Category Women => new(CategoryCode.WM, nameof(Women), [
        WomenTops, WomenBottoms, WomenDresses, WomenOuterwear, WomenAccessories
    ]);

    public static Category Kids => new(CategoryCode.KD, nameof(Kids), [KidsBoys, KidsGirls, KidsInfants]);
    public static Category Footwear => new(CategoryCode.FT, nameof(Footwear), []);
    public static Category Accessories => new(CategoryCode.AC, nameof(Accessories), []);
    public static Category Sale => new(CategoryCode.SL, nameof(Sale), []);

    // Men
    public static Category MenTops => new(CategoryCode.MT, "MenTops", []);
    public static Category MenBottoms => new(CategoryCode.MB, "MenBottoms", []);
    public static Category MenActivewear => new(CategoryCode.MA, "MenActivewear", []);
    public static Category MenAccessories => new(CategoryCode.MX, "MenAccessories", []);

    // Women
    public static Category WomenTops => new(CategoryCode.WT, "WomenTops", []);
    public static Category WomenBottoms => new(CategoryCode.WB, "WomenBottoms", []);
    public static Category WomenDresses => new(CategoryCode.WD, "WomenDresses", []);
    public static Category WomenOuterwear => new(CategoryCode.WO, "WomenOuterwear", []);
    public static Category WomenAccessories => new(CategoryCode.WX, "WomenAccessories", []);

    // Kids
    public static Category KidsBoys => new(CategoryCode.KB, "KidsBoys", []);
    public static Category KidsGirls => new(CategoryCode.KG, "KidsGirls", []);
    public static Category KidsInfants => new(CategoryCode.KI, "KidsInfants", []);

    // Functional helpers
    public static Fin<Category> FromCode(string code)
    {
        return Enum.TryParse<CategoryCode>(code, out var categoryCode)
            ? Optional(_all.FirstOrDefault(c => c.Code == categoryCode))
                .ToFin((Error)$"Invalid category code '{code}'")
            : FinFail<Category>((Error)$"Invalid category code '{code}'");
    }

    public static Fin<Category> FromName(string name)
    {
        return Optional(_all.FirstOrDefault(c => c.Name == name))
            .ToFin((Error)$"Invalid category name '{name}'");
    }

    public static Category FromUnsafe(string code)
    {
        return Enum.TryParse<CategoryCode>(code, out var categoryCode)
            ? Optional(_all.FirstOrDefault(c => c.Code == categoryCode)).Match(
                c => c, () => None)
            : None;
    }
}