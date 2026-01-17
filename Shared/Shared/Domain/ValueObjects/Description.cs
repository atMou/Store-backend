namespace Shared.Domain.ValueObjects;

public record Description
{
    public readonly string Value;

    private Description()
    {

    }
    private Description(string repr)
    {
        Value = repr;
    }
    public static Fin<Description> From(int min, int max, string repr) =>
       (from v in Helpers.MinLength(min, nameof(Description))(repr) &
                  Helpers.MaxLength(max, nameof(Description))(repr)
        select new Description(repr)).ToFin();

    public string To() => Value;


    public static Description FromUnsafe(string repr)
    {
        return new Description(repr);
    }
}
