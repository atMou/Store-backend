namespace Shared.Domain.ValueObjects;

public record AddressId : IId
{
	private AddressId(Guid value)
	{
		Value = value;
	}
	public Guid Value { get; init; }

	public static AddressId From(Guid value) => new(value);
	public static AddressId New => new(Guid.NewGuid());
	public virtual bool Equals(AddressId? other)
	{
		return other is not null && Value.Equals(other.Value);
	}
	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}
}
