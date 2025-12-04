

using Microsoft.Extensions.Logging;

using Shared.Application.Features.Inventory.Events;

namespace Product.Application.EventHandlers;

public class StockAddedIntegrationEventHandler(ProductDBContext dbContext, ILogger<StockAddedIntegrationEventHandler> logger)
    : IConsumer<StockAddedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<StockAddedIntegrationEvent> context)
    {
        var db = GetUpdateEntity<ProductDBContext, Domain.Models.Product>(
            p => p.Id == ProductId.From(context.Message.ProductId),
            NotFoundError.New($"Product with ID {context.Message.ProductId} not found."),
            null,
            p => p.UpdateStock(VariantId.From(context.Message.VariantId), context.Message.Stock, context.Message.Low, context.Message.Mid, context.Message.High)
        );


        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        result.IfFail(
            e => logger.LogError($"Failed to add stock to product with Id: {context.Message.ProductId}, {e}", e));


    }
}
