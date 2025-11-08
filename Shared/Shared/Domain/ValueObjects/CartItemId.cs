namespace Shared.Domain.ValueObjects;

public class CartItemId : IId
{
    private CartItemId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static CartItemId New()
    {
        return new CartItemId(Guid.NewGuid());
    }
    public static CartItemId From(Guid value)
    {
        return new CartItemId(value);
    }
}

