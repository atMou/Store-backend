using MassTransit;

using Microsoft.Extensions.Logging;

using Product.Application.Features.UpdateStock;

using Shared.Application.Features.Inventory.Events;

namespace Product.Application.EventHandlers;
internal class ProductReservedIntegrationEventHandler(ISender sender, ILogger<ProductReservedIntegrationEventHandler> logger) : IConsumer<ProductReservedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductReservedIntegrationEvent> context)
    {
        var result = await sender.Send(new UpdateStockCommand
        {
            ProductId = context.Message.ProductId,
            Stock = -context.Message.Stock,
            StockLevel = context.Message.StockLevel
        }, context.CancellationToken);
        result.IfFail(err => logger.LogError(err, "Failed to update stock for product {ProductId}", context.Message.ProductId));
    }
}
