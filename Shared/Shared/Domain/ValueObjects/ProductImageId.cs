namespace Shared.Domain.ValueObjects;

public record ProductImageId : IId
{
    private ProductImageId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static ProductImageId New => new ProductImageId(Guid.NewGuid());

    public static ProductImageId From(Guid value)
    {
        return new ProductImageId(value);
    }
}