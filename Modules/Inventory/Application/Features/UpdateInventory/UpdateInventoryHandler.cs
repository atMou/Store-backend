using Inventory.Application.Contracts;

using Shared.Domain.Errors;

namespace Inventory.Application.Features.UpdateInventory;

public record UpdateInventoryCommand : ICommand<Fin<Unit>>
{
    public Guid Id { get; init; }
    public IEnumerable<UpdateInventoryColorCommand> ColorVariants { get; init; }
}

public record UpdateInventoryColorCommand
{
    public Guid ColorVariantId { get; set; }
    public IEnumerable<UpdateInventorySizeCommand> SizeVariants { get; init; }
}

public record UpdateInventorySizeCommand
{
    public Guid? SizeVariantId { get; init; }
    public string Size { get; init; }
    public int Stock { get; init; }
    public int Low { get; init; }
    public int Mid { get; init; }
    public int High { get; init; }
    public IEnumerable<string> Warehouses { get; init; }
}

internal class UpdateInventoryCommandHandler(InventoryDbContext dbContext, IPublishEndpoint endpoint)
    : ICommandHandler<UpdateInventoryCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(UpdateInventoryCommand command, CancellationToken cancellationToken)
    {

        var db = GetUpdateEntity<InventoryDbContext, Domain.Models.Inventory>(
            inventory => inventory.Id == InventoryId.From(command.Id),
            NotFoundError.New($"Inventory with ID {command.Id} not found."),
            opt =>
            {
                opt = opt.AddInclude(i => i.ColorVariants);
                return opt;
            },
            inventory =>
            {
                return inventory.Update(command.ToDto()).Map(_ => inventory);
            });

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
            .RaiseOnSuccess(async i =>
            {
                await endpoint.Publish(new InventoryUpdatedIntegrationEvent(i.ProductId.Value, i.ColorVariants.Select(variant =>
                    new UpdateColorVariantDto()
                    {
                        ColorVariantId = variant.ColorVariantId.Value, SizeVariants =
                            variant.SizeVariants.Select(sizeVariant =>
                                new UpdateSizeVariantDto()
                                {
                                    SizeVariantId = sizeVariant.Id,
                                    Stock = sizeVariant.Stock.Value,
                                    Size = sizeVariant.Size.Code.ToString(),
                                    Level = sizeVariant.StockLevel
                                })
                    })), cancellationToken);
                return unit;
            });

    }



}
