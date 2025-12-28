namespace Product.Domain.ValueObjects;


public sealed record Status
{
    public bool IsFeatured { get; private init; }
    public bool IsTrending { get; private init; }
    public bool IsBestSeller { get; private init; }
    public bool IsNew { get; private init; }
    private Status(bool isFeatured, bool isTrending, bool isBestSeller, bool isNew)
    {
        IsFeatured = isFeatured;
        IsTrending = isTrending;
        IsBestSeller = isBestSeller;
        IsNew = isNew;
    }

    public static readonly Status New =
    new Status(
            isFeatured: false,
            isTrending: false,
            isBestSeller: false,
            true
        );

    public Status Update(bool? isFeatured, bool? isTrending, bool? isBestSeller, bool? isNew)
    {
        return new Status(
            isFeatured: isFeatured ?? IsFeatured,
            isTrending: isTrending ?? IsTrending,
            isBestSeller: isBestSeller ?? IsBestSeller,
            isNew: isNew ?? IsNew
        );
    }


}
