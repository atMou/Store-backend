namespace Inventory.Domain.Events;

public record StockDecreasedDomainEvent(
    ProductId ProductId,
    int Qty,
    int StockValue
) : IDomainEvent;