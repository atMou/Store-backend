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
}
