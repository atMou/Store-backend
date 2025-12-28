using Shared.Application.Features.Inventory.Events;

namespace Basket.Application.EventHandlers;
internal class ProductOutOfStockIntegrationEventHandler : IConsumer<ProductOutOfStockIntegrationEvent>
{
	public Task Consume(ConsumeContext<ProductOutOfStockIntegrationEvent> context)
	{
		return Task.CompletedTask;

	}
}
