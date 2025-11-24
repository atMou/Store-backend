namespace Inventory.Domain.Events;

public record ProductLowStockAlertEvent(ProductId ProductId, int Value) : IDomainEvent
{
}