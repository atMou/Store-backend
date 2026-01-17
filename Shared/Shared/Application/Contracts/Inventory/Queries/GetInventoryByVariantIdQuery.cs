using Shared.Application.Contracts.Inventory.Results;

namespace Shared.Application.Contracts.Inventory.Queries;

public record GetInventoryByColorVariantIdQuery : IQuery<Fin<InventoryResult>>
{
    public InventoryId InventoryId { get; init; }
}


