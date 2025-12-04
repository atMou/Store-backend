using Inventory.Persistence;

using Shared.Application.Abstractions;
using Shared.Domain.Errors;
using Shared.Persistence.Db.Monad;

namespace Inventory.Application.Features.DecreaseStock;
internal record DecreaseStockCommand : ICommand<Fin<Unit>>
{
    public ProductId ProductId { get; set; }
    public int Quantity { get; init; }
}
internal class AddStockHandler(InventoryDbContext dbContext) : ICommandHandler<DecreaseStockCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(DecreaseStockCommand command, CancellationToken cancellationToken)
    {
        var db = GetUpdateEntity<InventoryDbContext, Domain.Models.Inventory>(
            inventory => inventory.ProductId == command.ProductId,
            NotFoundError.New($"Product with ID {command.ProductId} not found in inventory."),
            null,
            (inventory) => inventory.Decrease(command.Quantity))
            .Map(_ => unit);
        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}

