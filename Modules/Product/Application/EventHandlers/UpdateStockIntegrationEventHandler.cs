using Microsoft.Extensions.Logging;

using Shared.Application.Features.Inventory.Events;

namespace Product.Application.EventHandlers;

public class UpdateStockIntegrationEventHandler(
    ProductDBContext dbContext,
    ILogger<UpdateStockIntegrationEventHandler> logger)
    : IConsumer<InventoryUpdatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<InventoryUpdatedIntegrationEvent> context)
    {

        var db = GetUpdateEntity<ProductDBContext, Domain.Models.Product>(
            p => p.Id == ProductId.From(context.Message.ProductId),
            NotFoundError.New($"Product with ID {context.Message.ProductId} not found."),
            opt =>
            {
                opt.AsSplitQuery = true;
                opt = opt.AddInclude(p => p.ColorVariants);
                return opt;
            },
            p => p.UpdateColorVariants([.. context.Message.ColorVariants])
        );

        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        result.IfFail(e => logger.LogError($"Error updating product stock for ProductId: {context.Message.ProductId}, {e}", e));

    }
}
