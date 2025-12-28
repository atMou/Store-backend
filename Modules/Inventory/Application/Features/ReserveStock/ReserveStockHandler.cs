using Inventory.Persistence;

using Shared.Application.Abstractions;
using Shared.Domain.Errors;
using Shared.Persistence.Db.Monad;

namespace Inventory.Application.Features.ReserveStock;

public record ReserveStockCommand : ICommand<Fin<Unit>>
{
	public ProductId ProductId { get; set; }
	public int Quantity { get; init; }
}
internal class ReserveStockCommandHandler(InventoryDbContext dbContext) : ICommandHandler<ReserveStockCommand, Fin<Unit>>
{
	public async Task<Fin<Unit>> Handle(ReserveStockCommand command, CancellationToken cancellationToken)
	{
		var db = GetUpdateEntity<InventoryDbContext, Domain.Models.Inventory>(
			inventory => inventory.ProductId == command.ProductId,
			NotFoundError.New($"Product with ID {command.ProductId} not found in inventory."),
			null,
			(inventory) => inventory.Reserve(command.Quantity)).Map(_ => unit);
		return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
	}
}

