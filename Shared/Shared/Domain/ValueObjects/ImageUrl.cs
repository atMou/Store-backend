namespace Shared.Domain.ValueObjects;

public record ImageUrl
{
    private static readonly Regex _allowedExtensions =
        new(@"\.(jpg|jpeg|png|webp)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private ImageUrl()
    {
    }

    private ImageUrl(string value, string publicId)
    {
        Value = value;
        PublicId = publicId;
    }

    public string Value { get; }
    public string PublicId { get; }
    public static Fin<ImageUrl> From(string url, string publicId)
    {
        return Helpers.IsNullOrEmpty(url, nameof(ImageUrl)).ToFin()
            .Bind(_ => IsValid(url)
                ? FinSucc(new ImageUrl(url, publicId))
                : FinFail<ImageUrl>(
                    InvalidOperationError.New("Invalid image URL. Must be a valid HTTP/HTTPS URL ending with .jpg, .jpeg, or .png."))
                .As()
                );
    }

    public static ImageUrl FromUnsafe(string url, string publicId)
    {
        return new ImageUrl(url, publicId);
    }
    public static ImageUrl? FromNullable(string? repr, string publicId)
    {
        return repr is null ? null : new ImageUrl(repr, "");
    }

    public string To()
    {
        return Value;
    }

    private static bool IsValid(string? url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) return false;

        if (!_allowedExtensions.IsMatch(uri.AbsolutePath)) return false;

        return true;
    }

    public virtual bool Equals(ImageUrl? other)
    {
        return other is not null && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

}