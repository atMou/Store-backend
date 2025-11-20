namespace Shared.Domain.ValueObjects;

public record ProductImage
{
    private ProductImage() { }
    private ProductImage(ImageUrl imageUrl, string altText, bool isMain)
    {
        ImageUrl = imageUrl;
        AltText = altText;
        IsMain = isMain;
        Id = ProductImageId.New;
    }
    public ProductImageId Id { get; private init; }
    public ImageUrl ImageUrl { get; private init; }
    public string AltText { get; private init; }
    public bool IsMain { get; private init; }
    public ProductId ProductId { get; set; }

    public static Fin<ProductImage> From(string url, string altText, bool IsMain)
    {
        return ImageUrl.From(url).Map(imageUrl => new ProductImage(imageUrl, altText, IsMain));
    }

    public virtual bool Equals(ProductImage? other)
    {
        return other is not null && ImageUrl == other.ImageUrl;
    }
    public override int GetHashCode()
    {
        return ImageUrl.GetHashCode();
    }

    //private static Fin<Unit> ValidateAltText(string altText)
    //{
    //    if (string.IsNullOrWhiteSpace(altText))
    //    {
    //        return FinFail<Unit>((Error)"Alt text is required.");
    //    }

    //    if (altText.Length > 100)
    //    {
    //        return FinFail<Unit>(Error.New("Alt text must be 100 characters or less."));
    //    }

    //    return FinSucc(unit);
    //}

}
