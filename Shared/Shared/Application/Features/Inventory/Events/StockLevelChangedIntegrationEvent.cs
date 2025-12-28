using Shared.Domain.Enums;

namespace Shared.Application.Features.Inventory.Events;

public record StockLevelChangedIntegrationEvent(Guid ProductId, Guid VariantId, bool InStock, StockLevel Level) : IntegrationEvent
{

}