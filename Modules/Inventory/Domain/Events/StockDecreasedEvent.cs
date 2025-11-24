namespace Inventory.Domain.Events;

public class StockDecreasedEvent : IDomainEvent
{
    public StockDecreasedEvent(ProductId productId, int qty, int stockValue)
    {
        throw new NotImplementedException();
    }
}