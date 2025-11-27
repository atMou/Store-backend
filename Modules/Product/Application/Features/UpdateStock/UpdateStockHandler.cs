using Shared.Domain.Enums;

namespace Product.Application.Features.UpdateStock;

public record UpdateStockCommand : ICommand<Fin<Unit>>
{
    public ProductId ProductId { get; init; }
    public int Stock { get; init; }
    public StockLevel StockLevel { get; init; }
}
internal class UpdateStockCommandHandler(ProductDBContext dbContext) : ICommandHandler<UpdateStockCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(UpdateStockCommand command, CancellationToken cancellationToken)
    {
        return GetUpdateEntity<ProductDBContext, Domain.Models.Product>(
            p => p.Id == command.ProductId,
            NotFoundError.New($"Product with ID {command.ProductId} not found."),
            p => p.UpdateStock(command.Stock, command.StockLevel)
        ).RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}

