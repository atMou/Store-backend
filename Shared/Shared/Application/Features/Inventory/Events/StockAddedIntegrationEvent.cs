namespace Shared.Application.Features.Inventory.Events;

public record StockAddedIntegrationEvent(Guid ProductId, Guid VariantId, int Stock, int? Low = null, int? Mid = null, int? High = null) : IntegrationEvent
{

}