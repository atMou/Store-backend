namespace Shared.Domain.ValueObjects;

public record MaterialId : IId
{
	private MaterialId(Guid value)
	{
		Value = value;
	}

	public Guid Value { get; }

	public static MaterialId From(Guid value) => new(value);
	public static MaterialId New => new(Guid.NewGuid());
	public virtual bool Equals(MaterialId? other)
	{
		return other is not null && Value.Equals(other.Value);
	}
	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}
}
