using Shared.Domain.Enums;

namespace Inventory.Domain.Events;

public record ProductOutOfStockDomainEvent(ProductId ProductId, VariantId VariantId, string Sku, StockLevel StockLevel, string Message) : IDomainEvent;