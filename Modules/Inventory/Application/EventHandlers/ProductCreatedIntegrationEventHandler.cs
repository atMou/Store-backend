using Microsoft.Extensions.Logging;

using Shared.Application.Features.Product.Events;

namespace Inventory.Application.EventHandlers;

public class ProductCreatedIntegrationInventoryHandler(
    ILogger<ProductCreatedIntegrationInventoryHandler> logger, InventoryDbContext dbContext)
    : IConsumer<ProductCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        var db = AddEntity<InventoryDbContext, Domain.Models.Inventory>(

             Domain.Models.Inventory.Create(
                 ProductId.From(message.ProductId),
                 message.Brand,
                 message.Slug,
                 message.ImageUrl,
                 message.ColorVariants
             ));
        await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken))
            .RaiseOnFail(err => logger.LogError($"Error creating inventory for Product with ID: {message.ProductId}, {err}", err));
    }
}