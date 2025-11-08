namespace Shared.Domain.ValueObjects;

public record ProductId : IId
{
    private ProductId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static ProductId New()
    {
        return new ProductId(Guid.NewGuid());
    }
    public static ProductId From(Guid value)
    {
        return new ProductId(value);
    }
}
