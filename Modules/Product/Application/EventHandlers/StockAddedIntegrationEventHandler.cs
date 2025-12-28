

using Microsoft.Extensions.Logging;

using Product.Domain.Models;

using Shared.Application.Features.Inventory.Events;

namespace Product.Application.EventHandlers;

public class StockAddedIntegrationEventHandler(ProductDBContext dbContext, ILogger<StockAddedIntegrationEventHandler> logger)
    : IConsumer<StockLevelChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<StockLevelChangedIntegrationEvent> context)
    {
        var db = GetUpdateEntity<ProductDBContext, Domain.Models.Product>(
            p => p.Id == ProductId.From(context.Message.ProductId),
            NotFoundError.New($"Product with ID {context.Message.ProductId} not found."),
            opt =>
            {
                opt.AsSplitQuery = true;
                opt = opt.AddInclude(p => p.Variants);
                return opt;
            },
            p =>
            {
                var variants = p.Variants.AsIterable().Fold(Seq<Variant>(), (seq, variant) =>
                {
                    if (variant.Id.Value == context.Message.VariantId)
                    {
                        return seq.Add(variant.UpdateStock(context.Message.InStock, context.Message.Level));
                    }

                    return seq.Add(variant);
                });

                return p.UpdateVariantsStock(variants.AsEnumerable());
            }

        );


        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken));

        result.IfFail(
            e => logger.LogError($"Failed to add Stock to product with Id: {context.Message.ProductId}, {e}", e));

    }
}
