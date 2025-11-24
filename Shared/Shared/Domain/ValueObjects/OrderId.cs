namespace Shared.Domain.ValueObjects;

public record OrderId : IId
{
    private OrderId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static OrderId From(Guid value) => new(value);
    public static OrderId New => new(Guid.NewGuid());
    public virtual bool Equals(OrderId? other)
    {
        return other is not null && Value.Equals(other.Value);
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
