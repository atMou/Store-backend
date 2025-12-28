using Product.Domain.Enums;

namespace Product.Domain.ValueObjects;

public record Brand
{
    private static readonly List<Brand> _all = new();

    public BrandCode Code { get; }
    public string Name { get; }
    public string? Country { get; }
    public string? Website { get; }
    public static IReadOnlyList<Brand> All => _all;

    private Brand()
    {
    }

    private Brand(BrandCode code, string name, string? country = null, string? website = null)
    {
        Code = code;
        Name = name;
        Country = country;
        Website = website;
        _all.Add(this);
    }
    static Brand()
    {
        _ = Nike;
        _ = Adidas;
        _ = Hm;
        _ = Zara;
        _ = LouisVuitton;
        _ = Gucci;
        _ = Uniqlo;
        _ = Prada;
        _ = Chanel;
        _ = Levis;
        _ = Puma;
        _ = UnderArmour;
        _ = NewBalance;
        _ = Asics;
        _ = Reebok;
        _ = Converse;
        _ = Timberland;
        _ = NorthFace;
        _ = UnKnown;
    }

    // Define brands
    public static Brand Nike => new(BrandCode.NK, "Nike", "USA", "https://www.nike.com");
    public static Brand Adidas => new(BrandCode.AD, "Adidas", "Germany", "https://www.adidas.com");
    public static Brand Hm => new(BrandCode.HM, "H&M", "Sweden", "https://www2.hm.com");
    public static Brand Zara => new(BrandCode.ZR, "Zara", "Spain", "https://www.zara.com");
    public static Brand LouisVuitton => new(BrandCode.LV, "Louis Vuitton", "France", "https://www.louisvuitton.com");
    public static Brand Gucci => new(BrandCode.GU, "Gucci", "Italy", "https://www.gucci.com");
    public static Brand Uniqlo => new(BrandCode.UN, "Uniqlo", "Japan", "https://www.uniqlo.com");
    public static Brand Prada => new(BrandCode.PR, "Prada", "Italy", "https://www.prada.com");
    public static Brand Chanel => new(BrandCode.CH, "Chanel", "France", "https://www.chanel.com");
    public static Brand Levis => new(BrandCode.LVG, "Levi's", "USA", "https://www.levi.com");
    public static Brand Puma => new(BrandCode.PU, "Puma", "Germany", "https://www.puma.com");

    public static Brand UnderArmour => new(BrandCode.UA, "Under Armour",
        "USA", "https://www.underarmour.com");

    public static Brand NewBalance => new(BrandCode.NB, "New Balance", "USA", "https://www.newbalance.com");
    public static Brand Asics => new(BrandCode.AS, "Asics", "Japan", "https://www.asics.com");

    public static Brand Reebok => new(BrandCode.RB, "Reebok",
        "USA", "https://www.reebok.com");

    public static Brand Converse => new(BrandCode.CV, "Converse",
        "USA", "https://www.converse.com");

    public static Brand Timberland => new(BrandCode.TB, "Timberland",
        "USA", "https://www.timberland.com");

    public static Brand NorthFace => new(BrandCode.NF, "The North Face",
        "USA", "https://www.thenorthface.com");

    public static Brand UnKnown => new(BrandCode.UNK, nameof(UnKnown), nameof(UnKnown));


    public static Fin<Brand> From(string name)
    {
        return Optional(_all.FirstOrDefault(b => b.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            .ToFin(ValidationError.New($"Invalid brand name '{name}'"));
    }

    public static IEnumerable<Brand> Like(IEnumerable<string> repr)
    {
        return _all.Where(brand => repr.Any(s => brand.Name.Contains(s)));
    }

    public static Brand FromUnsafe(string repr)
    {
        return
            Optional(_all.FirstOrDefault(b => b.Name == repr))
                .Match(b => b, () => UnKnown);

    }
}