using Microsoft.Extensions.Logging;

using Shared.Application.Features.Product.Events;
using Shared.Infrastructure.Logging;

namespace Inventory.Application.EventHandlers;

public class ProductCreatedIntegrationEventHandler(InventoryDbContext dbContext, ILogger<ProductCreatedIntegrationEventHandler> logger) : IConsumer<ProductCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductCreatedIntegrationEvent> context)
    {

        var db = AddEntities<InventoryDbContext, Domain.Models.Inventory>(
            context.Message.Variants.Select(variant =>
                Domain.Models.Inventory.Create(ProductId.From(context.Message.ProductId),
                    VariantId.From(variant.VariantId), variant.Sku, variant.Stock, variant.Low, variant.Mid,
                    variant.High, context.Message.Brand, context.Message.Slug))
        );
        await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken))
            .RaiseOnFail(err => logger.LogIntegrationEventFailed(nameof(ProductCreatedIntegrationEvent), err));
    }
}
