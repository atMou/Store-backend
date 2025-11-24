namespace Shared.Domain.ValueObjects;

public record ReviewId : IId
{
    private ReviewId(Guid value)
    {
        Value = value;
    }
    public Guid Value { get; init; }

    public static ReviewId From(Guid value) => new(value);
    public static ReviewId New => new(Guid.NewGuid());
    public virtual bool Equals(ReviewId? other)
    {
        return other is not null && Value.Equals(other.Value);
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
