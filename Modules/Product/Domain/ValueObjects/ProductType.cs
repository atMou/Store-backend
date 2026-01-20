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


    static ProductType()
    {


        // --- MEN CLOTHING ---
        _ = MenKnitwear;
        _ = MenJacketsSidebar;
        _ = MenCoatsSidebar;
        _ = MenSweatshirtsHoodies;
        _ = MenShirtsSidebar;
        _ = MenTShirtsPolos;
        _ = MenTrousersSidebar;
        _ = MenJeansSidebar;
        _ = MenSuitsTailoring;
        _ = MenTracksuitsJoggers;
        _ = MenShortsSidebar;
        _ = MenUnderwearSocks;
        _ = MenLoungeSleepwear;
        _ = MenPlusSize;
        _ = MenAdaptiveFashion;
        _ = MenSwimwear;

        // --- MEN SHOES ---
        _ = MenTrainers;
        _ = MenBootsSidebar;
        _ = MenFormalShoes;
        _ = MenSandalsSlides;

        // --- MEN ACCESSORIES ---
        _ = MenBagsSidebar;
        _ = MenWatches;
        _ = MenWalletsCardHolders;
        _ = MenBeltsSidebar;
        _ = MenHatsCaps;
        _ = MenSunglasses;
        _ = MenJewellery;

        // --- WOMEN CLOTHING ---
        _ = WomenKnitwearCardigans;
        _ = WomenCoatsSidebar;
        _ = WomenJacketsSidebar;
        _ = WomenDresses;
        _ = WomenSkirtsSidebar;
        _ = WomenUnderwear;
        _ = WomenSwimwear;

        // --- WOMEN SHOES ---
        _ = WomenBootsSidebar;
        _ = WomenAnkleBoots;
        _ = WomenFlatShoes;
        _ = WomenTrainers;
        _ = WomenHighHeels;
        _ = WomenBalletPumps;
        _ = WomenPumps;
        _ = WomenMules;
        _ = WomenSlippers;
        _ = WomenSandals;

        // --- WOMEN ACCESSORIES ---
        _ = WomenBeanies;
        _ = WomenBalaclava;
        _ = WomenScarvesSidebar;
        _ = WomenGloves;
        _ = WomenBagsCases;
        _ = WomenJewellerySidebar;
        _ = WomenBelts;
        _ = WomenWatches;
        _ = WomenWallets;
        _ = WomenSunglasses;
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


    // --- MEN CLOTHING ---
    public static readonly ProductType MenKnitwear = new("Knitwear", ["Sweaters", "Cardigans", "Turtlenecks", "Pullovers"]);
    public static readonly ProductType MenJacketsSidebar = new("Jackets", ["Bomber", "Denim", "Leather", "Windbreaker", "Track"]);
    public static readonly ProductType MenCoatsSidebar = new("Coats", ["Overcoat", "Trench", "Puffer", "Wool", "Parka"]);
    public static readonly ProductType MenSweatshirtsHoodies = new("Sweatshirts & Hoodies", ["Hoodies", "Sweatshirts", "Zip Hoodies", "Crew Neck"]);
    public static readonly ProductType MenShirtsSidebar = new("Shirts", ["Casual", "Dress", "Denim", "Flannel", "Oxford"]);
    public static readonly ProductType MenTShirtsPolos = new("T-shirts & Polos", ["T-Shirts", "Polo", "Long Sleeve", "Graphic", "V-Neck"]);
    public static readonly ProductType MenTrousersSidebar = new("Trousers", ["Chinos", "Dress", "Cargo", "Casual", "Cropped"]);
    public static readonly ProductType MenJeansSidebar = new("Jeans", ["Slim", "Straight", "Skinny", "Regular", "Relaxed"]);
    public static readonly ProductType MenSuitsTailoring = new("Suits & Tailoring", ["Suits", "Blazers", "Suit Trousers", "Waistcoats", "Tuxedos"]);
    public static readonly ProductType MenTracksuitsJoggers = new("Tracksuits & Joggers", ["Tracksuits", "Joggers", "Track Pants", "Sweatpants"]);
    public static readonly ProductType MenShortsSidebar = new("Shorts", ["Casual", "Denim", "Sports", "Chino", "Cargo"]);
    public static readonly ProductType MenUnderwearSocks = new("Underwear & Socks", ["Boxers", "Briefs", "Socks", "Undershirts", "Long Johns"]);
    public static readonly ProductType MenLoungeSleepwear = new("Lounge- & Sleepwear", ["Pajamas", "Lounge Pants", "Robes", "Sleep Shorts"]);
    public static readonly ProductType MenPlusSize = new("Plus size", ["Plus Size"]);
    public static readonly ProductType MenAdaptiveFashion = new("Adaptive Fashion", ["Adaptive"]);
    public static readonly ProductType MenSwimwear = new("Swimwear", ["Trunks", "Board Shorts", "Briefs", "Rash Guards"]);

    // --- MEN SHOES ---
    public static readonly ProductType MenTrainers = new("Trainers", ["Running", "Casual", "High Tops", "Low Tops", "Slip-On"]);
    public static readonly ProductType MenBootsSidebar = new("Boots", ["Chelsea", "Desert", "Work", "Combat", "Hiking"]);
    public static readonly ProductType MenFormalShoes = new("Formal Shoes", ["Oxfords", "Derby", "Loafers", "Brogues", "Monk Straps"]);
    public static readonly ProductType MenSandalsSlides = new("Sandals & Slides", ["Slides", "Flip Flops", "Sport", "Leather"]);

    // --- MEN ACCESSORIES ---
    public static readonly ProductType MenBagsSidebar = new("Bags", ["Backpacks", "Messenger", "Duffel", "Briefcases", "Tote"]);
    public static readonly ProductType MenWatches = new("Watches", ["Analog", "Digital", "Smart", "Sport"]);
    public static readonly ProductType MenWalletsCardHolders = new("Wallets & Card Holders", ["Wallets", "Card Holders", "Money Clips"]);
    public static readonly ProductType MenBeltsSidebar = new("Belts", ["Leather", "Canvas", "Dress", "Casual"]);
    public static readonly ProductType MenHatsCaps = new("Hats & Caps", ["Baseball", "Beanies", "Bucket", "Fedoras", "Snapbacks"]);
    public static readonly ProductType MenSunglasses = new("Sunglasses", ["Aviator", "Wayfarer", "Round", "Square", "Sport"]);
    public static readonly ProductType MenJewellery = new("Jewellery", ["Necklaces", "Bracelets", "Rings", "Earrings"]);

    // --- WOMEN CLOTHING ---
    public static readonly ProductType WomenKnitwearCardigans = new("Knitwear & Cardigans", ["Sweaters", "Cardigans", "Turtlenecks", "Pullovers"]);
    public static readonly ProductType WomenCoatsSidebar = new("Coats", ["Trench", "Puffer", "Wool", "Peacoat"]);
    public static readonly ProductType WomenJacketsSidebar = new("Jackets", ["Denim", "Leather", "Bomber", "Blazers"]);
    public static readonly ProductType WomenDresses = new("Dresses", ["Casual", "Evening", "Maxi", "Midi", "Mini"]);
    public static readonly ProductType WomenSkirtsSidebar = new("Skirts", ["Mini", "Midi", "Maxi", "Pencil", "A-Line"]);
    public static readonly ProductType WomenUnderwear = new("Underwear", ["Bras", "Panties", "Sets", "Shapewear"]);
    public static readonly ProductType WomenSwimwear = new("Swimwear", ["Bikini", "One-Piece", "Tankini", "Cover-Ups"]);

    // --- WOMEN SHOES ---
    public static readonly ProductType WomenBootsSidebar = new("Boots", ["Ankle", "Knee High", "Over-the-Knee", "Chelsea", "Combat"]);
    public static readonly ProductType WomenAnkleBoots = new("Ankle boots", ["Heeled", "Flat", "Wedge"]);
    public static readonly ProductType WomenFlatShoes = new("Flat shoes", ["Ballet", "Loafers", "Oxfords", "Moccasins"]);
    public static readonly ProductType WomenTrainers = new("Trainers", ["Running", "Casual", "High Tops", "Slip-On"]);
    public static readonly ProductType WomenHighHeels = new("High heels", ["Stiletto", "Block", "Kitten", "Platform"]);
    public static readonly ProductType WomenBalletPumps = new("Ballet pumps", ["Classic", "Pointed", "Round"]);
    public static readonly ProductType WomenPumps = new("Pumps", ["Classic", "Pointed", "Peep Toe"]);
    public static readonly ProductType WomenMules = new("Mules", ["Heeled", "Flat", "Backless"]);
    public static readonly ProductType WomenSlippers = new("Slippers", ["House", "Slide", "Moccasin"]);
    public static readonly ProductType WomenSandals = new("Sandals", ["Flat", "Heeled", "Gladiator", "Wedge"]);

    // --- WOMEN ACCESSORIES ---
    public static readonly ProductType WomenBeanies = new("Beanies", ["Knit", "Slouchy", "Pom"]);
    public static readonly ProductType WomenBalaclava = new("Balaclava", ["Winter", "Fashion"]);
    public static readonly ProductType WomenScarvesSidebar = new("Scarves", ["Silk", "Wool", "Infinity", "Pashmina"]);
    public static readonly ProductType WomenGloves = new("Gloves", ["Leather", "Knit", "Touchscreen"]);
    public static readonly ProductType WomenBagsCases = new("Bags & cases", ["Handbags", "Shoulder", "Crossbody", "Tote", "Clutch"]);
    public static readonly ProductType WomenJewellerySidebar = new("Jewellery", ["Necklaces", "Bracelets", "Earrings", "Rings"]);
    public static readonly ProductType WomenBelts = new("Belts", ["Leather", "Chain", "Fabric"]);
    public static readonly ProductType WomenWatches = new("Watches", ["Analog", "Digital", "Smart"]);
    public static readonly ProductType WomenWallets = new("Wallets & card holders", ["Wallets", "Card Holders", "Coin Purses"]);
    public static readonly ProductType WomenSunglasses = new("Sunglasses", ["Aviator", "Cat Eye", "Round", "Oversized"]);


}