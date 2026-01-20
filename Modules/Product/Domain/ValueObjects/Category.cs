namespace Product.Domain.ValueObjects;

public record Category
{
    private const string Men = "Men";
    private const string Women = "Women";

    private const string Shoes = "Shoes";
    private const string Accessories = "Accessories";
    private const string Clothing = "Clothing";

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

        _ = MenClothing;
        _ = MenShoesSidebar;
        _ = MenAccessoriesSidebar;

        _ = WomenClothing;
        _ = WomenShoesSidebar;
        _ = WomenAccessoriesSidebar;
    }

    public string Main { get; }
    public string Sub { get; }
    [NotMapped]
    public IEnumerable<ProductType> ProductTypes { get; }




    public static Category MenClothing => new(Men, Clothing,
    [
        ProductType.MenKnitwear,
        ProductType.MenJacketsSidebar,
        ProductType.MenCoatsSidebar,
        ProductType.MenSweatshirtsHoodies,
        ProductType.MenShirtsSidebar,
        ProductType.MenTShirtsPolos,
        ProductType.MenTrousersSidebar,
        ProductType.MenJeansSidebar,
        ProductType.MenSuitsTailoring,
        ProductType.MenTracksuitsJoggers,
        ProductType.MenShortsSidebar,
        ProductType.MenUnderwearSocks,
        ProductType.MenLoungeSleepwear,
        ProductType.MenPlusSize,
        ProductType.MenAdaptiveFashion,
        ProductType.MenSwimwear
    ]);

    public static Category MenShoesSidebar => new(Men, Shoes,
    [
        ProductType.MenTrainers,
        ProductType.MenBootsSidebar,
        ProductType.MenFormalShoes,
        ProductType.MenSandalsSlides
    ]);

    public static Category MenAccessoriesSidebar => new(Men, Accessories,
    [
        ProductType.MenBagsSidebar,
        ProductType.MenWatches,
        ProductType.MenWalletsCardHolders,
        ProductType.MenBeltsSidebar,
        ProductType.MenHatsCaps,
        ProductType.MenSunglasses,
        ProductType.MenJewellery
    ]);

    public static Category WomenClothing => new(Women, Clothing,
    [
        ProductType.WomenKnitwearCardigans,
        ProductType.WomenCoatsSidebar,
        ProductType.WomenJacketsSidebar,
        ProductType.WomenDresses,
        ProductType.WomenSkirtsSidebar,
        ProductType.WomenUnderwear,
        ProductType.WomenSwimwear
    ]);

    public static Category WomenShoesSidebar => new(Women, Shoes,
    [
        ProductType.WomenBootsSidebar,
        ProductType.WomenAnkleBoots,
        ProductType.WomenFlatShoes,
        ProductType.WomenTrainers,
        ProductType.WomenHighHeels,
        ProductType.WomenBalletPumps,
        ProductType.WomenPumps,
        ProductType.WomenMules,
        ProductType.WomenSlippers,
        ProductType.WomenSandals
    ]);

    public static Category WomenAccessoriesSidebar => new(Women, Accessories,
    [
        ProductType.WomenBeanies,
        ProductType.WomenBalaclava,
        ProductType.WomenScarvesSidebar,
        ProductType.WomenGloves,
        ProductType.WomenBagsCases,
        ProductType.WomenJewellerySidebar,
        ProductType.WomenBelts,
        ProductType.WomenWatches,
        ProductType.WomenWallets,
        ProductType.WomenSunglasses
    ]);

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
        return $"{Main.First()}-{Sub.First()}";
    }
}