namespace Inventory.Domain.Events;

public record ProductBackInStockDomainEvent(ProductId ProductId, ColorVariantId ColorVariantId, int Value) : IDomainEvent;