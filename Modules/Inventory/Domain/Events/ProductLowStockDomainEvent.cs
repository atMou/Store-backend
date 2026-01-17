namespace Inventory.Domain.Events;

public record ProductLowStockDomainEvent(ProductId ProductId, ColorVariantId ColorVariantId, int Value) : IDomainEvent
{
}