namespace Inventory.Domain.Events;

public record ProductOutOfStockAlertEvent(ProductId ProductId) : IDomainEvent;