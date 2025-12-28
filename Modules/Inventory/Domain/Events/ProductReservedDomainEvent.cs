namespace Inventory.Domain.Events;

public record ProductReservedDomainEvent(ProductId ProductId, VariantId VariantId, string Sku, int Qty, int AvailableStock) : IDomainEvent
{

}