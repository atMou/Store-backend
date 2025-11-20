using System.Numerics;

namespace Shared.Domain.ValueObjects;

public record Email : DomainType<Email, string>,
    IEqualityOperators<Email, string, bool>


{
    public readonly string Value;

    private Email(string repr)
    {
        Value = repr;
    }
    public static Fin<Email> From(string repr)
    {
        return IsValidEmail(repr)
            ? FinSucc(new Email(repr))
            : FinFail<Email>(Errors.Errors.Domain.Users.Email.Invalid(repr));
    }



    public string To()
    {
        return Value;
    }

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static bool IsValidEmail(string email) =>
        !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);

    public static bool operator ==(Email? left, string? right)
    {
        return left is { } e && right is { } r && string.Equals(e.Value, r, StringComparison.OrdinalIgnoreCase);
    }

    public static bool operator !=(Email? left, string? right)
    {
        return !(left == right);
    }


    public static Email FromUnsafe(string repr)
    {
        return new Email(repr);
    }
}
