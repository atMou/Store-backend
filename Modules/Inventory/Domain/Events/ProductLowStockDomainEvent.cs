namespace Inventory.Domain.Events;

public record ProductLowStockDomainEvent(ProductId ProductId, VariantId VariantId, string Sku, int Value) : IDomainEvent
{
}