namespace Shared.Domain.ValueObjects;

public record InventoryId : IId
{
    private InventoryId() => Value = Guid.Empty;
    private InventoryId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static InventoryId New()
    {
        return new InventoryId(Guid.NewGuid());
    }
    public static InventoryId From(Guid value)
    {
        return new InventoryId(value);
    }
    public virtual bool Equals(InventoryId? other)
    {
        return other is not null && Value.Equals(other.Value);
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
