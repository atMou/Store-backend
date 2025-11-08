namespace Shared.Domain.ValueObjects;

public record UserId : IId
{
    public Guid Value { get; init; }
    private UserId(Guid value)
    {
        Value = value;
    }

    public static UserId From(Guid value) => new(value);
    public static UserId New => new(Guid.NewGuid());
}
