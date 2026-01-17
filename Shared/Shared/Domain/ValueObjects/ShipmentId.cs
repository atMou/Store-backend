namespace Shared.Domain.ValueObjects;

public record ShipmentId : IId
{
    private ShipmentId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static ShipmentId From(Guid value) => new(value);
    public static ShipmentId New => new(Guid.NewGuid());
    public virtual bool Equals(ShipmentId? other)
    {
        return other is not null && Value.Equals(other.Value);
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
