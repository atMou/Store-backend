using Shared.Domain.Enums;

namespace Inventory.Domain.Events;

public record ProductOutOfStockDomainEvent(
    ProductId ProductId,
    string Color,
    string Size,
    string Message,
    StockLevel StockLevel,
    bool IsAvailable,
    int Stock,
    string Slug
    ) : IDomainEvent;