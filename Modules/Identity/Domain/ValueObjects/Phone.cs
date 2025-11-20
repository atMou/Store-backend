using System.Text.RegularExpressions;

namespace Identity.Domain.ValueObjects;

public record Phone : DomainType<Phone, string>
{
    public string Value { get; }
    private Phone(string value)
    {
        Value = value;
    }

    public static Fin<Phone> From(string repr) =>
        repr switch
        {
            { Length: 0 } or null => FinFail<Phone>(ValidationError.New("Phone number cannot be empty.")),
            { Length: > 0 } when !Regex.IsMatch(repr) => FinFail<Phone>(ValidationError.New("Invalid phone number format.")),
            _ => FinSucc<Phone>(new Phone(repr))
        };

    public string To()
    {
        return Value;
    }

    public static Phone FromUnsafe(string repr)
    {
        return new Phone(repr);
    }
    public static Phone? FromNullable(string? repr)
    {
        return repr is null ? null : new Phone(repr);
    }


    private static readonly Regex Regex = new(@"^\+\d{1,3}\d{9,12}$", RegexOptions.Compiled);


    public static implicit operator string(Phone phone)
    {
        return phone.To();
    }
}
