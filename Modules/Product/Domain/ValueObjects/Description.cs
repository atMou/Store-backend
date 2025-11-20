
namespace Product.Domain.ValueObjects;

public record Description : DomainType<Description, string>
{
    public readonly string Value;

    private Description()
    {

    }
    private Description(string repr)
    {
        Value = repr;
    }
    public static Fin<Description> From(string repr) =>
       (from v in Helpers.MinLength(50, nameof(Description))(repr) &
                  Helpers.MaxLength(200, nameof(Description))(repr)
        select new Description(repr)).ToFin();

    public string To() => Value;


    public static Description FromUnsafe(string repr)
    {
        return new Description(repr);
    }
}
