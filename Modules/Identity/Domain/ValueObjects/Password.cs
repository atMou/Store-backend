
using Shared.Domain.Errors;
using Shared.Domain.Validations;

namespace Identity.ValueObjects;

using LanguageExt.Traits.Domain;

public record Password : DomainType<Password, string>
{
    public readonly string Value;

    private Password(string value)
    {
        Value = value;
    }
    public static Fin<Password> From(string repr)
    {
        return IsValid(repr).Map(_ => new Password(repr)).ToFin();
    }

    public string To()
    {
        return Value;
    }


    public static Validation<Error, Unit> IsValid(string repr)
    {
        return from x in Helpers.IsNullOrEmpty(repr, nameof(Password)) &
                           Helpers.MaxLength20(repr, nameof(Password)) &
                           Helpers.MinLength8(repr, nameof(Password)) &
                            HasUpper(repr) & HasSpecialChar(repr)
               select unit;
    }

    public static Validation<Error, Unit> HasUpper(string repr)
    {
        return repr.Any(char.IsUpper)
            ? Success<Error, Unit>(unit)
            : ValidationError.New("Password must contain at least one uppercase letter.");
    }

    public static Validation<Error, Unit> HasSpecialChar(string repr)
    {
        return (!repr.Any(c => "!@#$%^&*()_+-=[]{}|;:'\",.<>?/`~".Contains(c)))
            ? ValidationError.New("Password must contain at least one special character.")
            : unit;
    }

}
