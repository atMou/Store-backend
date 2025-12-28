using Shared.Domain.Errors;

namespace Inventory.Application.Features.AddStock;

public record AddStockCommand : ICommand<Fin<Unit>>
{
	public VariantId VariantId { get; init; }
	public int Stock { get; init; }

}
internal class AddStockCommandHandler(InventoryDbContext dbContext, IPublishEndpoint endpoint) : ICommandHandler<AddStockCommand, Fin<Unit>>
{
	public async Task<Fin<Unit>> Handle(AddStockCommand command, CancellationToken cancellationToken)
	{

		var db = GetUpdateEntity<InventoryDbContext, Domain.Models.Inventory>(inventory => inventory.VariantId == command.VariantId,
			NotFoundError.New($"Variant with ID {command.VariantId.Value} not found."),
			null,
			inventory => inventory.Increase(command.Stock)
			).Map(_ => unit);

		return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
	}
}