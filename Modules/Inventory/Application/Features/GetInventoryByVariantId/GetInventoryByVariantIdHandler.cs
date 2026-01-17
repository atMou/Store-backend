using Inventory.Application.Contracts;

using Shared.Application.Contracts.Inventory.Queries;
using Shared.Application.Contracts.Inventory.Results;
using Shared.Domain.Errors;

namespace Inventory.Application.Features.GetInventoryByVariantId;

internal class GetInventoryByVariantIdQueryHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetInventoryByColorVariantIdQuery, Fin<InventoryResult>>
{
    public async Task<Fin<InventoryResult>> Handle(GetInventoryByColorVariantIdQuery query, CancellationToken cancellationToken)
    {
        var inventoryId = query.InventoryId;

        var db = GetEntity<InventoryDbContext, Domain.Models.Inventory>(
            inventory => inventory.Id == inventoryId,
            NotFoundError.New($"Inventory with ID {inventoryId.Value} not found."),
            opt =>
            {
                opt.AsNoTracking = true;
                opt = opt.AddInclude(i => i.ColorVariants);
                return opt;
            }
        ).Map(inventory => inventory.ToResult());

        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
