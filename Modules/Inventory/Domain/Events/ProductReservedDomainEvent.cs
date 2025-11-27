using Shared.Domain.Enums;

namespace Inventory.Domain.Events;

public record ProductReservedDomainEvent(ProductId ProductId, int Qty, StockLevel StockLevel) : IDomainEvent
{

}