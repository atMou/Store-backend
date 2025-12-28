namespace Inventory.Domain.Events;

public record ProductBackInStockDomainEvent(ProductId ProductId, VariantId VariantId, string Sku, int Value) : IDomainEvent;