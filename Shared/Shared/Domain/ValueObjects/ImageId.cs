namespace Shared.Domain.ValueObjects;

public record ImageId : IId
{
    private ImageId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static ImageId New => new ImageId(Guid.NewGuid());

    public static ImageId From(Guid value)
    {
        return new ImageId(value);
    }
}