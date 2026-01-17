namespace Shared.Application.Features.Inventory.Events;

public record ProductOutOfStockIntegrationEvent(Guid ProductId, string Color, string Size, string Slug) : IntegrationEvent;