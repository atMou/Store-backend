namespace Inventory.Domain.Events;

public record ProductReservedDomainEvent(ProductId ProductId, ColorVariantId ColorVariantId, int Qty, int AvailableStock) : IDomainEvent
{

}