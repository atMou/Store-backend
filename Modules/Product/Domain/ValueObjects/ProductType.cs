namespace Product.Domain.ValueObjects;

public sealed record ProductType
{
    private ProductType() { }

    private ProductType(string type, string[] allowedSubTypes)
    {
        Type = type;
        AllowedSubTypes = allowedSubTypes;
        _all.Add(this);
    }

    public string Type { get; }
    public string SubType { get; init; }
    public string[] AllowedSubTypes { get; }
    public static IReadOnlyList<ProductType> All => _all;

    private static readonly List<ProductType> _all = [];

    // MEN — TOPS
    public static readonly ProductType MenTShirts = new("T-Shirts",
        ["Crew Neck", "V-Neck", "Polo", "Graphic", "Long Sleeve"]);

    public static readonly ProductType MenShirts = new("Shirts", ["Formal", "Casual", "Checked", "Denim", "Linen"]);

    public static readonly ProductType MenPolos = new("Polos",
        ["Short Sleeve", "Long Sleeve", "Slim Fit", "Regular Fit", "Striped"]);

    public static readonly ProductType MenSweaters =
        new("Sweaters", ["Crew Neck", "V-Neck", "Hooded", "Cardigan", "Cable Knit"]);

    // MEN — BOTTOMS
    public static readonly ProductType MenJeans = new("Jeans",
        ["Slim Fit", "Straight Leg", "Relaxed Fit", "Skinny", "Tapered"]);

    public static readonly ProductType MenTrousers =
        new("Men Trousers", ["Chinos", "Tailored", "Cargo", "Slim Fit", "Cropped"]);

    public static readonly ProductType
        MenShorts = new("Shorts", ["Denim", "Cargo", "Athletic", "Bermuda", "Chino"]);

    public static readonly ProductType MenSweatpants =
        new("Sweatpants", ["Joggers", "Track", "Slim Fit", "Loose Fit", "Cuffed"]);

    // MEN — OUTERWEAR
    public static readonly ProductType MenJackets = new("Jackets", ["Bomber", "Denim", "Leather", "Field"]);
    public static readonly ProductType MenCoats = new("Coats", ["Overcoat", "Trench", "Peacoat", "Topcoat"]);

    public static readonly ProductType MenBlazers = new("Blazers",
        ["Single-Breasted", "Double-Breasted", "Casual", "Structured"]);

    public static readonly ProductType MenParkas = new("Parkas", ["Insulated", "Rain Parka", "Hooded", "Fur Trim"]);

    // MEN — SHOES
    public static readonly ProductType MenSneakers = new("Sneakers", ["Low Top", "High Top", "Retro", "Trainer"]);
    public static readonly ProductType MenBoots = new("Boots", ["Chelsea", "Work", "Hiker", "Chukka"]);
    public static readonly ProductType MenLoafers = new("Loafers", ["Penny", "Tassel", "Bit", "Boat"]);
    public static readonly ProductType MenSandals = new("Sandals", ["Slide", "Sport", "Thong", "Fisherman"]);

    // MEN — ACCESSORIES
    public static readonly ProductType MenBelts = new("Belts", ["Leather", "Canvas", "Braided", "Reversible"]);
    public static readonly ProductType MenHats = new("Hats", ["Cap", "Beanie", "Fedora", "Bucket"]);
    public static readonly ProductType MenBags = new("Bags", ["Backpack", "Messenger", "Duffle", "Crossbody"]);
    public static readonly ProductType MenScarves = new("Scarves", ["Wool", "Silk", "Infinity", "Lightweight"]);

    // WOMEN — TOPS
    public static readonly ProductType WomenTShirts =
        new("T-Shirts", ["Crew Neck", "V-Neck", "Oversized", "Graphic", "Cropped"]);

    public static readonly ProductType
        WomenBlouses = new("Blouses", ["Chiffon", "Silk", "Wrap", "Peplum", "Button-Up"]);

    public static readonly ProductType WomenShirts =
        new("Shirts", ["Formal", "Casual", "Oversized", "Denim", "Linen"]);

    public static readonly ProductType WomenKnitwear =
        new("Knitwear", ["Crew Neck", "V-Neck", "Cardigan", "Turtleneck", "Cable Knit"]);

    // WOMEN — BOTTOMS
    public static readonly ProductType WomenJeans = new("Women Jeans",
        ["Skinny", "Straight Leg", "Wide Leg", "High Waist", "Mom Fit"]);

    public static readonly ProductType WomenTrousers =
        new("Trousers", ["Tailored", "Wide Leg", "Cropped", "Pleated", "High Waist"]);

    public static readonly ProductType WomenSkirts = new("Skirts", ["Mini", "Midi", "Maxi", "Pleated", "Wrap"]);

    public static readonly ProductType WomenShorts =
        new("Shorts", ["Denim", "High Waist", "Tailored", "Casual", "Cycling"]);

    // WOMEN — OUTERWEAR
    public static readonly ProductType WomenJackets = new("Jackets", ["Bomber", "Denim", "Leather", "Tailored"]);
    public static readonly ProductType WomenCoats = new("Coats", ["Trench", "Wool", "Wrap", "Puffer"]);

    public static readonly ProductType WomenBlazers =
        new("Blazers", ["Single-Breasted", "Oversized", "Structured", "Casual"]);

    public static readonly ProductType WomenParkas =
        new("Parkas", ["Insulated", "Rain Parka", "Hooded", "Fur Trim"]);

    // WOMEN — SHOES
    public static readonly ProductType WomenHeels = new("Heels", ["Stiletto", "Block", "Kitten", "Wedge"]);
    public static readonly ProductType WomenFlats = new("Flats", ["Ballet", "Loafer", "D’Orsay", "Slip-On"]);
    public static readonly ProductType WomenBoots = new("Boots", ["Ankle", "Knee-High", "Chelsea", "Combat"]);

    public static readonly ProductType WomenSneakers =
        new("Sneakers", ["Fashion", "Trainer", "Platform", "Slip-On"]);

    // WOMEN — ACCESSORIES
    public static readonly ProductType WomenBags = new("Bags", ["Shoulder", "Tote", "Crossbody", "Clutch"]);
    public static readonly ProductType WomenScarves = new("Scarves", ["Silk", "Wool", "Pashmina", "Lightweight"]);
    public static readonly ProductType WomenHats = new("Hats", ["Sun", "Beret", "Fedora", "Beanie"]);

    public static readonly ProductType WomenJewelry =
        new("Jewelry", ["Necklace", "Earrings", "Bracelet", "Rings"]);

    // BOY
    public static readonly ProductType BoyTShirts =
        new("T-Shirts", ["Graphic", "Striped", "Printed", "Long Sleeve"]);

    public static readonly ProductType BoyShirts = new("Shirts", ["Casual", "Checked", "School", "Denim"]);
    public static readonly ProductType BoyShorts = new("Shorts", ["Cargo", "Denim", "Athletic", "Chino"]);

    public static readonly ProductType BoyOuterwear =
        new("Outerwear", ["Hoodie", "Puffer", "Rain Jacket", "Bomber"]);

    // GIRL
    public static readonly ProductType GirlTops = new("Tops", ["Graphic", "Ruffle", "Blouse", "Tank"]);
    public static readonly ProductType GirlDresses = new("Dresses", ["Casual", "Party", "Floral", "Maxi"]);
    public static readonly ProductType GirlSkirts = new("Skirts", ["Tutu", "Denim", "Pleated", "Mini"]);

    public static readonly ProductType GirlOuterwear =
        new("Outerwear", ["Cardigan", "Puffer", "Denim Jacket", "Raincoat"]);

    // BABY
    public static readonly ProductType BabyOnesies =
        new("Onesies", ["Short Sleeve", "Long Sleeve", "Snap", "Organic"]);

    public static readonly ProductType BabySleepwear =
        new("Sleepwear", ["Sleep Suit", "Footed Pajamas", "Swaddle", "Gown"]);

    public static readonly ProductType BabySets = new("Sets", ["2-piece", "3-piece", "Gift Set", "Seasonal"]);

    public static readonly ProductType BabyAccessories =
        new("Accessories", ["Booties", "Hats", "Bibs", "Blankets"]);

    static ProductType()
    {
        // MEN
        _ = MenTShirts;
        _ = MenShirts;
        _ = MenPolos;
        _ = MenSweaters;
        _ = MenJeans;
        _ = MenTrousers;
        _ = MenShorts;
        _ = MenSweatpants;
        _ = MenJackets;
        _ = MenCoats;
        _ = MenBlazers;
        _ = MenParkas;
        _ = MenSneakers;
        _ = MenBoots;
        _ = MenLoafers;
        _ = MenSandals;
        _ = MenBelts;
        _ = MenHats;
        _ = MenBags;
        _ = MenScarves;

        // WOMEN
        _ = WomenTShirts;
        _ = WomenBlouses;
        _ = WomenShirts;
        _ = WomenKnitwear;
        _ = WomenJeans;
        _ = WomenTrousers;
        _ = WomenSkirts;
        _ = WomenShorts;
        _ = WomenJackets;
        _ = WomenCoats;
        _ = WomenBlazers;
        _ = WomenParkas;
        _ = WomenHeels;
        _ = WomenFlats;
        _ = WomenBoots;
        _ = WomenSneakers;
        _ = WomenBags;
        _ = WomenScarves;
        _ = WomenHats;
        _ = WomenJewelry;

        // BOY / GIRL / BABY
        _ = BoyTShirts;
        _ = BoyShirts;
        _ = BoyShorts;
        _ = BoyOuterwear;
        _ = GirlTops;
        _ = GirlDresses;
        _ = GirlSkirts;
        _ = GirlOuterwear;
        _ = BabyOnesies;
        _ = BabySleepwear;
        _ = BabySets;
        _ = BabyAccessories;
    }
    public static Fin<ProductType> From(string type, string subType) =>
        Optional(_all.FirstOrDefault(pt => pt.Type.Equals(type, StringComparison.OrdinalIgnoreCase)
                                           && pt.AllowedSubTypes.Contains(subType))).Map(productType => productType with { SubType = subType })
            .ToFin(ValidationError.New($"Invalid product type '{type}' of sub type '{subType}'"));
    public bool Equals(ProductType? other)
    {
        return other is not null &&
               Type.Equals(other.Type, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type.ToLowerInvariant(), SubType.ToLowerInvariant());
    }
}