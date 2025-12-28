namespace Product.Domain.ValueObjects;

public record Category
{
    private const string Men = "Men";
    private const string Women = "Women";
    private const string Baby = "Baby";
    private const string Boy = "Boy";
    private const string Girl = "Girl";


    private const string Tops = "Tops";
    private const string Bottoms = "Bottoms";
    private const string Outerwear = "Outerwear";
    private const string Shoes = "Shoes";
    private const string Accessories = "Accessories";
    private const string Sleepwear = "Sleepwear";

    private static readonly List<Category> _all = [];

    private Category() { }
    private Category(string main, string sub, IEnumerable<ProductType> productTypes)
    {
        Main = main;
        Sub = sub;

        ProductTypes = productTypes ?? [];
        _all.Add(this);
    }

    static Category()
    {
        // NONE
        _ = None;

        // MEN
        _ = MenTops;
        _ = MenBottoms;
        _ = MenOuterwear;
        _ = MenShoes;
        _ = MenAccessories;

        // WOMEN
        _ = WomenTops;
        _ = WomenBottoms;
        _ = WomenOuterwear;
        _ = WomenShoes;
        _ = WomenAccessories;

        // BOY
        _ = BoyTops;
        _ = BoyBottoms;
        _ = BoyOuterwear;
        _ = BoySleepwear;
        _ = BoyShoes;
        _ = BoyAccessories;

        // GIRL
        _ = GirlTops;
        _ = GirlBottoms;
        _ = GirlOuterwear;
        _ = GirlSleepwear;
        _ = GirlShoes;
        _ = GirlAccessories;

        // BABY
        _ = BabyTops;
        _ = BabyBottoms;
        _ = BabyOuterwear;
        _ = BabySleepwear;
        _ = BabyShoes;
        _ = BabyAccessories;
    }

    public string Main { get; }
    public string Sub { get; }
    [NotMapped]
    public IEnumerable<ProductType> ProductTypes { get; }

    public static Category None => new("", "", []);


    public static Category MenTops => new(Men, Tops,
        [ProductType.MenTShirts, ProductType.MenShirts, ProductType.MenPolos, ProductType.MenSweaters]);
    public static Category MenBottoms => new(Men, Bottoms,
        [ProductType.MenJeans, ProductType.MenShorts, ProductType.MenSweatpants, ProductType.MenTrousers]);
    public static Category MenOuterwear => new(Men, Outerwear,
        [ProductType.MenJackets, ProductType.MenCoats, ProductType.MenBlazers, ProductType.MenParkas]);
    public static Category MenShoes => new(Men, Shoes,
        [ProductType.MenSneakers, ProductType.MenBoots, ProductType.MenLoafers, ProductType.MenSandals]);
    public static Category MenAccessories => new(Men, Accessories,
        [ProductType.MenBelts, ProductType.MenHats, ProductType.MenBags, ProductType.MenScarves]);

    public static Category WomenTops => new(Women, Tops,
        [ProductType.WomenTShirts, ProductType.WomenBlouses, ProductType.WomenShirts, ProductType.WomenKnitwear]);
    public static Category WomenBottoms => new(Women, Bottoms,
        [ProductType.WomenJeans, ProductType.WomenTrousers, ProductType.WomenSkirts, ProductType.WomenShorts]);
    public static Category WomenOuterwear => new(Women, Outerwear,
        [ProductType.WomenJackets, ProductType.WomenCoats, ProductType.WomenBlazers, ProductType.WomenParkas]);
    public static Category WomenShoes => new(Women, Shoes,
        [ProductType.WomenHeels, ProductType.WomenFlats, ProductType.WomenBoots, ProductType.WomenSneakers]);
    public static Category WomenAccessories => new(Women, Accessories,
        [ProductType.WomenBags, ProductType.WomenScarves, ProductType.WomenHats, ProductType.WomenJewelry]);

    // BOY
    public static Category BoyTops => new(Boy, Tops,
        [ProductType.BoyTShirts, ProductType.BoyShirts, ProductType.BoyOuterwear]);
    public static Category BoyBottoms => new(Boy, Bottoms,
        [ProductType.BoyShorts, ProductType.BoyShorts]);
    public static Category BoyOuterwear => new(Boy, Outerwear,
        [ProductType.BoyOuterwear, ProductType.BoyTShirts, ProductType.BoyShirts]);
    public static Category BoySleepwear => new(Boy, Sleepwear,
        [ProductType.BoyOuterwear, ProductType.BabySleepwear]);
    public static Category BoyShoes => new(Boy, Shoes,
        [ProductType.BabyAccessories]);
    public static Category BoyAccessories => new(Boy, Accessories,
        [ProductType.BabyAccessories, ProductType.BoyShirts]);

    // GIRL
    public static Category GirlTops => new(Girl, Tops,
        [ProductType.GirlTops, ProductType.GirlDresses, ProductType.GirlSkirts]);
    public static Category GirlBottoms => new(Girl, Bottoms,
        [ProductType.GirlSkirts, ProductType.GirlDresses, ProductType.GirlOuterwear]);
    public static Category GirlOuterwear => new(Girl, Outerwear,
        [ProductType.GirlOuterwear, ProductType.GirlTops, ProductType.GirlSkirts]);
    public static Category GirlSleepwear => new(Girl, Sleepwear,
        [ProductType.BabySleepwear, ProductType.GirlTops]);
    public static Category GirlShoes => new(Girl, Shoes,
        [ProductType.BabyAccessories]);
    public static Category GirlAccessories => new(Girl, Accessories,
        [ProductType.BabyAccessories, ProductType.GirlTops]);

    // BABY
    public static Category BabyTops => new(Baby, Tops,
        [ProductType.BabyOnesies, ProductType.BabySets, ProductType.BabySleepwear]);
    public static Category BabyBottoms => new(Baby, Bottoms,
        [ProductType.BabySets, ProductType.BabyAccessories]);
    public static Category BabyOuterwear => new(Baby, Outerwear,
        [ProductType.BabyOnesies, ProductType.BabySleepwear]);
    public static Category BabySleepwear => new(Baby, Sleepwear,
        [ProductType.BabySleepwear, ProductType.BabyOnesies]);
    public static Category BabyShoes => new(Baby, Shoes,
        [ProductType.BabyAccessories, ProductType.BabySets]);
    public static Category BabyAccessories => new(Baby, Accessories,
        [ProductType.BabyAccessories, ProductType.BabyOnesies]);

    public static IReadOnlyList<Category> All => _all;

    public static Func<Fin<ProductType>, Fin<Category>> From(string main, string sub)
    {
        return type =>
            Optional(_all.FirstOrDefault(c => c.Main == main && c.Sub == sub))
            .ToFin(ValidationError.New($"Invalid category name '{sub}'"))
            .Bind(category => type.Bind(productType => category.HasType(productType)));
    }

    public static IEnumerable<string> SubLiKe(IEnumerable<string> repr)
    {
        return _all.Where(category =>
            repr.Any(s =>
                category.Sub.Equals(s, StringComparison.OrdinalIgnoreCase)
            )
        ).Select(c => c.Sub).Distinct();
    }


    private Fin<Category> HasType(ProductType type)
    {
        return ProductTypes.Contains(type)
            ? Fin<Category>.Succ(this)
            : ValidationError.New($"Category '{ToString()}' does not contain product type '{type.Type}-{type.SubType}'");
    }

    //public static Category FromUnsafe(string repr) =>
    //    _all.FirstOrDefault(c => c.Identifier == repr) ?? None;

    public virtual bool Equals(Category? other) =>
        other is { } o && Main == o.Main && Sub == o.Sub;

    public override int GetHashCode() => HashCode.Combine(Main, Sub);
    public override string ToString()
    {
        return $"{Main}-{Sub}";
    }
}