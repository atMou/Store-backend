using Shared.Domain.Enums;

namespace Shared.Application.Features.Inventory.Events;

public record ProductReservedIntegrationEvent(ProductId ProductId, int Stock, StockLevel StockLevel)
{

}