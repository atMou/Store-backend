namespace Inventory.Domain.Events;

public record ProductBackInStockDomainEvent(ProductId ProductId, int Value) : IDomainEvent;