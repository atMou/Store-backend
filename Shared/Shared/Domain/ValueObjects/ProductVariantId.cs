namespace Shared.Domain.ValueObjects;

public record VariantId : IId
{
	private VariantId(Guid value)
	{
		Value = value;
	}
	public Guid Value { get; init; }

	public static VariantId From(Guid value) => new(value);
	public static VariantId New => new(Guid.NewGuid());
	public virtual bool Equals(VariantId? other)
	{
		return other is not null && Value.Equals(other.Value);
	}
	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}
}
