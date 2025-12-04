using Inventory.Persistence;

using MassTransit;

using Shared.Application.Abstractions;
using Shared.Application.Features.Inventory.Events;
using Shared.Persistence.Db.Monad;

namespace Inventory.Application.Features.AddStock;

public record CreateStockCommand : ICommand<Fin<Unit>>
{
    public ProductId ProductId { get; init; }
    public VariantId VariantId { get; init; }
    public int Quantity { get; init; }
    public int StockLow { get; init; }
    public int StockMid { get; init; }
    public int StockHigh { get; init; }
}
internal class CreateStockCommandHandler(InventoryDbContext dbContext, IPublishEndpoint endpoint) : ICommandHandler<CreateStockCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(CreateStockCommand command, CancellationToken cancellationToken)
    {
        var entity = Domain.Models.Inventory.Create(command.ProductId, command.Quantity, command.StockLow,
            command.StockMid, command.StockHigh);
        var db = AddEntity<InventoryDbContext, Domain.Models.Inventory>(entity);

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async i =>
        {
            var stockAddedEvent = new StockAddedIntegrationEvent(i.ProductId.Value, command.VariantId.Value, i.Stock.Value);
            await endpoint.Publish(stockAddedEvent, cancellationToken);

            return unit;
        });
    }
}