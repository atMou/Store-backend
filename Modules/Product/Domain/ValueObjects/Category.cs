namespace Product.Domain.ValueObjects;

public record Category
{
    private static readonly List<Category> _all = [];
    public readonly IEnumerable<Category> Subcategories;

    private Category() { }

    private Category(CategoryCode code, string name, IEnumerable<Category> subcategories)
    {
        Code = code;
        Name = name;
        Subcategories = subcategories ?? [];
        _all.Add(this);
    }

    static Category()
    {
        _ = None;

        _ = Men;
        _ = Women;
        _ = Kids;

        _ = NewIn;
        _ = Trending;
        _ = Sale;
        _ = Sports;
        _ = Designer;
        _ = Brands;
        _ = Beauty;

        _ = Accessories;
        _ = Footwear;
        _ = Shoes;
        _ = Handbags;

        _ = Tops;
        _ = Bottoms;
        _ = Dresses;
        _ = Outerwear;
        _ = Sweaters;
        _ = Suits;
        _ = Activewear;
        _ = Underwear;

        _ = Blouses;
        _ = Lingerie;
        _ = Skirts;
        _ = Coats;

        _ = Sleepwear;
        _ = Jackets;
        _ = TShirts;
        _ = Jeans;
        _ = Shorts;
        _ = Pants;

        _ = Toys;
    }

    public CategoryCode Code { get; }
    public string Name { get; }
    public static IReadOnlyList<Category> All => [Men, Women, Kids, NewIn, Trending, Sale, Sports, Designer, Brands, Beauty];


    public static Category None => new(CategoryCode.None, "None", []);

    public static Category Men => new(CategoryCode.Men, nameof(Men), [
        Tops, Bottoms, Outerwear, Activewear, Suits, Underwear,
        Shoes, Accessories
    ]);

    public static Category Women => new(CategoryCode.Women, nameof(Women), [
        Tops, Bottoms, Dresses, Outerwear, Sweaters, Lingerie,
        Skirts, Handbags, Shoes, Accessories, Beauty
    ]);

    public static Category Kids => new(CategoryCode.Kids, nameof(Kids), [
        Clothing, Toys, Footwear, Sleepwear
    ]);

    public static Category NewIn => new(CategoryCode.NewIn, nameof(NewIn), []);
    public static Category Trending => new(CategoryCode.Trending, nameof(Trending), []);
    public static Category Sale => new(CategoryCode.Sale, nameof(Sale), []);
    public static Category Sports => new(CategoryCode.Sports, nameof(Sports), []);
    public static Category Designer => new(CategoryCode.Designer, nameof(Designer), []);
    public static Category Brands => new(CategoryCode.Brands, nameof(Brands), []);
    public static Category Beauty => new(CategoryCode.Beauty, nameof(Beauty), []);

    public static Category Clothing => new(CategoryCode.Clothing, "Clothing", []);

    public static Category Accessories => new(CategoryCode.Accessories, "Accessories", []);
    public static Category Footwear => new(CategoryCode.Footwear, "Footwear", []);
    public static Category Shoes => new(CategoryCode.Shoes, "Shoes", []);
    public static Category Handbags => new(CategoryCode.Handbags, "Handbags", []);

    public static Category Tops => new(CategoryCode.Tops, "Tops", []);
    public static Category Bottoms => new(CategoryCode.Bottoms, "Bottoms", []);
    public static Category Dresses => new(CategoryCode.Dresses, "Dresses", []);
    public static Category Outerwear => new(CategoryCode.Outerwear, "Outerwear", []);
    public static Category Sweaters => new(CategoryCode.Sweaters, "Sweaters", []);
    public static Category Suits => new(CategoryCode.Suits, "Suits", []);
    public static Category Activewear => new(CategoryCode.Activewear, "Activewear", []);
    public static Category Underwear => new(CategoryCode.Underwear, "Underwear", []);

    public static Category Blouses => new(CategoryCode.Blouses, "Blouses", []);
    public static Category Lingerie => new(CategoryCode.Lingerie, "Lingerie", []);
    public static Category Skirts => new(CategoryCode.Skirts, "Skirts", []);
    public static Category Coats => new(CategoryCode.Coats, "Coats", []);

    public static Category Toys => new(CategoryCode.Toys, "Toys", []);
    public static Category Sleepwear => new(CategoryCode.Sleepwear, "Sleepwear", []);

    public static Category Jackets => new(CategoryCode.Jackets, "Jackets", []);
    public static Category TShirts => new(CategoryCode.TShirts, "TShirts", []);
    public static Category Jeans => new(CategoryCode.Jeans, "Jeans", []);
    public static Category Shorts => new(CategoryCode.Shorts, "Shorts", []);
    public static Category Pants => new(CategoryCode.Pants, "Pants", []);


    public static Fin<Category> From(string repr)
    {
        return Enum.TryParse<CategoryCode>(repr, out var categoryCode)
            ? Optional(_all.FirstOrDefault(c => c.Code == categoryCode))
                .ToFin((Error)$"Invalid category code '{repr}'")
            : FinFail<Category>((Error)$"Invalid category code '{repr}'");
    }

    public static Category FromUnsafe(string repr)
        => Enum.TryParse<CategoryCode>(repr, out var code)
            ? _all.FirstOrDefault(c => c.Code == code) ?? None
            : None;

    public virtual bool Equals(Category? other)
        => other is { } o && Code == o.Code;

    public override int GetHashCode()
        => Code.GetHashCode();
}
