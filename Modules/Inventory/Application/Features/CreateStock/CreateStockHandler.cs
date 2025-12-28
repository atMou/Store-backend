
using Shared.Domain.Enums;

namespace Inventory.Application.Features.CreateStock;

public record CreateStockCommand : ICommand<Fin<Unit>>
{
    public ProductId ProductId { get; init; }
    public VariantId VariantId { get; init; }
    public string Sku { get; set; }
    public string Brand { get; set; }
    public string Slug { get; set; }
    public int Stock { get; init; }
    public int Low { get; init; }
    public int Mid { get; init; }
    public int High { get; init; }
}
internal class CreateStockCommandHandler(InventoryDbContext dbContext, IPublishEndpoint endpoint) : ICommandHandler<CreateStockCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(CreateStockCommand command, CancellationToken cancellationToken)
    {
        var entity = Domain.Models.Inventory.Create(
            command.ProductId,
            command.VariantId,
            command.Sku,
            command.Stock,
            command.Low,
            command.Mid,
            command.High,
            command.Brand,
            command.Slug
            );
        var db = AddEntity<InventoryDbContext, Domain.Models.Inventory>(entity);

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async i =>
        {
            var stockAddedEvent = new StockLevelChangedIntegrationEvent(i.ProductId.Value, i.VariantId.Value, i.StockLevel >= StockLevel.MediumStock, i.StockLevel);
            await endpoint.Publish(stockAddedEvent, cancellationToken);

            return unit;
        });
    }
}