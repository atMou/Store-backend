namespace Shared.Domain.ValueObjects;

public record LineItemId : IId
{
    private LineItemId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static LineItemId New =>
        new LineItemId(Guid.NewGuid());
    public static LineItemId From(Guid value)
    {
        return new LineItemId(value);
    }
    public virtual bool Equals(LineItemId? other)
    {
        return other is not null && Value.Equals(other.Value);
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

