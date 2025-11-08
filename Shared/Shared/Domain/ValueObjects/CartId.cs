namespace Shared.Domain.ValueObjects;

public record CartId : IId
{
    private CartId(Guid value)
    {
        Value = value;
    }
    public Guid Value { get; init; }

    public static CartId From(Guid value) => new(value);
    public static CartId New => new(Guid.NewGuid());
}
