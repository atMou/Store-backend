namespace Shared.Domain.ValueObjects;

public record PaymentId : IId
{
    private PaymentId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static PaymentId From(Guid value) => new(value);
    public static PaymentId New => new(Guid.NewGuid());
}
