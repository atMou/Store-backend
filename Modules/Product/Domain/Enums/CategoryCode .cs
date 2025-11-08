namespace Product.Domain.Enums;
public enum CategoryCode
{
    None,
    // Top-level categories
    MN, // Men
    WM, // Women
    KD, // Kids
    FT, // Footwear
    AC, // Accessories
    SL, // Sale

    // Men subcategories
    MT, // MenTops
    MB, // MenBottoms
    MA, // MenActivewear
    MX, // MenAccessories

    // Women subcategories
    WT, // WomenTops
    WB, // WomenBottoms
    WD, // WomenDresses
    WO, // WomenOuterwear
    WX, // WomenAccessories

    // Kids subcategories
    KB, // KidsBoys
    KG, // KidsGirls
    KI  // KidsInfants
}
