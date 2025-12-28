namespace Order.Domain.ValueObjects;
public record OrderItemId : IId
{
	private OrderItemId(Guid value)
	{
		Value = value;
	}

	public Guid Value { get; }

	public static OrderItemId New => new(Guid.NewGuid());

	public static OrderItemId From(Guid value)
	{
		return new OrderItemId(value);
	}
}
