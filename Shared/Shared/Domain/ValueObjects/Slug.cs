namespace Shared.Domain.ValueObjects;

public record Slug : DomainType<Slug, string>
{
	public readonly string Value;

	private Slug()
	{

	}
	private Slug(string repr)
	{
		Value = repr;
	}
	public static Fin<Slug> From(string repr) =>
		(from v in Helpers.MinLength(5, nameof(Slug))(repr) &
				   Helpers.MaxLength(50, nameof(Slug))(repr)
		 select new Slug(repr)).ToFin();


	public static Slug FromUnsafe(string repr)
	{
		return new Slug(repr);
	}


	public string To() => Value;
}
