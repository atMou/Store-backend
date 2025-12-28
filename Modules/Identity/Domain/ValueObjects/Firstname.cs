using Shared.Domain.Validations;

namespace Identity.ValueObjects;

using System.Numerics;

using LanguageExt.Traits.Domain;

public record Firstname : DomainType<Firstname, string>, IEqualityOperators<Firstname, string, bool>
{
	public readonly string Value;

	private Firstname(string repr)
	{
		Value = repr;
	}
	public static Fin<Firstname> From(string repr)
	{
		return (from _ in Helpers.IsNullOrEmpty(repr, nameof(Firstname)) &
							  Helpers.IsNullOrWhiteSpace(repr, nameof(Firstname)) &
							  Helpers.MaxLength50(repr, nameof(Firstname))
				select new Firstname(repr)).ToFin();
	}

	public string To()
	{
		return Value;
	}


	public static bool operator ==(Firstname? left, string? right)
	{
		return left is { } l && right is { } r && String.Equals(l.Value, r, StringComparison.OrdinalIgnoreCase);
	}

	public static bool operator !=(Firstname? left, string? right)
	{
		return !(left == right);
	}

	public static Firstname FromUnsafe(string repr)
	{
		return new Firstname(repr);
	}
}
