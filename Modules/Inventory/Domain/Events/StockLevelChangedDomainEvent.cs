using Shared.Domain.Enums;

namespace Inventory.Domain.Events;

public sealed record StockLevelChangedDomainEvent(Guid ProductId, Guid VariantId, string Size, bool InStock, StockLevel Level) : IDomainEvent
{
}