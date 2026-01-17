using Shared.Domain.Errors;

namespace Inventory.Application.Features.DeleteInventory;

public record DeleteStockCommand : ICommand<Fin<Unit>>
{
    public Guid Id { get; init; }
}

internal class DeleteStockCommandHandler(InventoryDbContext dbContext) : ICommandHandler<DeleteStockCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(DeleteStockCommand command, CancellationToken cancellationToken)
    {
        var db = HardDeleteEntity<InventoryDbContext, Domain.Models.Inventory>(
            inventory => inventory.Id == InventoryId.From(command.Id),
            NotFoundError.New($"Inventory with Variant ID {command.Id} not found.")
        ).Map(_ => unit);

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
