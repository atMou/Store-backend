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
    public VariantId VariantId { get; set; }

    public static Fin<ProductImage> From(string url, string altText, bool IsMain)
    {
        return ImageUrl.From(url).Map(imageUrl => new ProductImage(imageUrl, altText, IsMain));
    }

    public static ProductImage FromUnsafe(string url, string altText, bool IsMain)
    {
        return new ProductImage(ImageUrl.FromUnsafe(url), altText, IsMain);
    }
    public Fin<ProductImage> Update(string url, string altText, bool isMain)
    {
        return ImageUrl.From(url).Map(imageUrl => this with
        {
            ImageUrl = imageUrl,
            AltText = altText,
            IsMain = isMain
        });
    }

    public virtual bool Equals(ProductImage? other)
    {
        return other is not null && ImageUrl == other.ImageUrl;
    }
    public override int GetHashCode()
    {
        return ImageUrl.GetHashCode();
    }


}
