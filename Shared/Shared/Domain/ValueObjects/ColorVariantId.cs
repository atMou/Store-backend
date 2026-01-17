namespace Shared.Domain.ValueObjects;

public record ColorVariantId : IId
{
    private ColorVariantId(Guid value)
    {
        Value = value;
    }
    public Guid Value { get; init; }

    public static ColorVariantId From(Guid value) => new(value);
    public static ColorVariantId New => new(Guid.NewGuid());
    public virtual bool Equals(ColorVariantId? other)
    {
        return other is not null && Value.Equals(other.Value);
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
