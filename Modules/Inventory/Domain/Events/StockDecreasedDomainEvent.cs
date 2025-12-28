namespace Inventory.Domain.Events;

public class StockDecreasedDomainEvent : IDomainEvent
{
	public StockDecreasedDomainEvent(ProductId productId, int qty, int stockValue)
	{
		throw new NotImplementedException();
	}
}