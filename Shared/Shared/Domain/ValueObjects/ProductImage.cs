namespace Shared.Domain.ValueObjects;

public record Image
{
    private Image() { }
    private Image(ImageUrl imageUrl, string altText, bool isMain)
    {
        ImageUrl = imageUrl;
        AltText = altText;
        IsMain = isMain;
        Id = ImageId.New;
    }
    public ImageId Id { get; private init; }
    public ImageUrl ImageUrl { get; private init; }
    public string AltText { get; private set; }
    public bool IsMain { get; private set; }
    public ProductId ProductId { get; set; }
    public ColorVariantId ColorVariantId { get; set; }

    public static Fin<Image> From(string url, string publicId, string altText, bool IsMain)
    {
        return ImageUrl.From(url, publicId).Map(imageUrl => new Image(imageUrl, altText, IsMain));
    }

    public static Image FromUnsafe(string url, string publicId, string altText, bool IsMain)
    {
        return new Image(ImageUrl.FromUnsafe(url, publicId), altText, IsMain);
    }
    public Image Update(string altText, bool isMain)
    {
        if (altText != AltText || IsMain != isMain)
        {
            AltText = altText;
            IsMain = isMain;
        }
        return this;
    }

    public virtual bool Equals(Image? other)
    {
        return other is not null && ImageUrl.Value == other.ImageUrl.Value;
    }
    public override int GetHashCode()
    {
        return ImageUrl.GetHashCode();
    }


}
