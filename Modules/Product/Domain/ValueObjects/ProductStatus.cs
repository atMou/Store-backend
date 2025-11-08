namespace Product.Domain.ValueObjects;


public record ProductStatus
{
    public bool IsFeatured { get; private init; }
    public bool IsTrending { get; private init; }
    public bool IsBestSeller { get; private init; }
    public bool IsNew { get; private init; }
    private ProductStatus(bool isFeatured, bool isTrending, bool isBestSeller, bool isNew)
    {
        IsFeatured = isFeatured;
        IsTrending = isTrending;
        IsBestSeller = isBestSeller;
        IsNew = isNew;
    }

    public static readonly ProductStatus New =
    new ProductStatus(
            isFeatured: false,
            isTrending: false,
            isBestSeller: false,
            true
        );

    public ProductStatus Update(bool? isFeatured, bool? isTrending, bool? isBestSeller, bool? isNew) =>
        new(isFeatured: isFeatured ?? IsFeatured,
            isTrending: isTrending ?? IsTrending,
            isBestSeller: isBestSeller ?? IsBestSeller,
            isNew: isNew ?? IsNew
        );

}
