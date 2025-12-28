namespace Shared.Application.Features.Inventory.Events;

public record ProductOutOfStockIntegrationEvent(Guid ProductId, Guid VariantId) : IntegrationEvent;