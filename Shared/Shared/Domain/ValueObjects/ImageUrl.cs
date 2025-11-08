using Shared.Domain.Validations;

namespace Shared.Domain.ValueObjects;

public record ImageUrl
{
    private static readonly Regex _allowedExtensions =
        new(@"\.(jpg|jpeg|png)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private ImageUrl() { }

    private ImageUrl(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Fin<ImageUrl> From(string url)
    {
        return Helpers.IsNullOrEmpty(url, nameof(ImageUrl)).ToFin()
            .Bind(_ => IsValid(url)
                ? FinSucc(new ImageUrl(url!))
                : FinFail<ImageUrl>(
                    (Error)"Invalid image URL. Must be a valid HTTP/HTTPS URL ending with .jpg, .jpeg, or .png.")
                .As()
                );
    }


    private static bool IsValid(string? url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) return false;

        if (!_allowedExtensions.IsMatch(uri.AbsolutePath)) return false;

        return true;
    }
}