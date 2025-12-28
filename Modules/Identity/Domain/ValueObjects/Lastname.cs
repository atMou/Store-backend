using Shared.Domain.Validations;

namespace Identity.ValueObjects;

using System.Numerics;

using LanguageExt.Traits.Domain;

public record Lastname : DomainType<Lastname, string>, IEqualityOperators<Lastname, string, bool>
{
	public readonly string Value;

	private Lastname(string value)
	{
		Value = value;
	}

	public static Fin<Lastname> From(string repr)
	{
		return (from _ in Helpers.IsNullOrEmpty(repr, nameof(Lastname)) &
						  Helpers.IsNullOrWhiteSpace(repr, nameof(Lastname)) &
						  Helpers.MaxLength50(repr, nameof(Lastname))
				select new Lastname(repr)).ToFin();
	}

	public string To()
	{
		return Value;
	}


	public static bool operator ==(Lastname? left, string? right)
	{
		return left is { } l && right is { } r && String.Equals(l.Value, r, StringComparison.OrdinalIgnoreCase);
	}

	public static bool operator !=(Lastname? left, string? right)
	{
		return !(left == right);
	}


	public static Lastname FromUnsafe(string repr)
	{
		return new Lastname(repr);
	}
}
