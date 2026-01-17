using Inventory.Application.Contracts;

using Shared.Application.Contracts;
using Shared.Application.Contracts.Inventory.Results;

namespace Inventory.Application.Features.GetInventories;

public record GetInventoriesQuery : IQuery<Fin<PaginatedResult<InventoryResult>>>, IPagination
{
    public string? Search { get; init; }
    public string? Brand { get; init; }
    public string? Size { get; init; }
    public string? WarehouseCode { get; init; }
    public string? OrderBy { get; init; }
    public string? SortDir { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

internal class GetInventoriesQueryHandler(InventoryDbContext dbContext)
    : IQueryHandler<GetInventoriesQuery, Fin<PaginatedResult<InventoryResult>>>
{
    public async Task<Fin<PaginatedResult<InventoryResult>>> Handle(GetInventoriesQuery query, CancellationToken cancellationToken)
    {
        var db = GetEntitiesWithPagination<InventoryDbContext, Domain.Models.Inventory, InventoryResult, GetInventoriesQuery>(
            null,
            options =>
            {
                options.AsNoTracking = true;
                options = options.AddInclude(i => i.ColorVariants);
                return QueryEvaluator.Evaluate(options, query);
            },
            query,
            inventory => inventory.ToResult()
        );

        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
