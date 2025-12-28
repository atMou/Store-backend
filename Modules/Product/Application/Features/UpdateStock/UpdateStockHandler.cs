namespace Product.Application.Features.UpdateStock;

//public record UpdateStockCommand : ICommand<Fin<Unit>>
//{
//    public ProductId ProductId { get; init; }
//    public VariantId VariantId { get; init; }
//    public int Stock { get; init; }
//    public StockLevel StockLevel { get; init; }
//}
//internal class UpdateStockCommandHandler(ProductDBContext dbContext) : ICommandHandler<UpdateStockCommand, Fin<Unit>>
//{
//    public async Task<Fin<Unit>> Handle(UpdateStockCommand command, CancellationToken cancellationToken)
//    {
//        return await GetUpdateEntity<ProductDBContext, Domain.Models.Product>(
//            p => p.Id == command.ProductId,
//            NotFoundError.New($"Product with ID {command.ProductId} not found."),
//            null,
//            p => p.UpdateStock(command.VariantId, command.Stock)
//        ).Map(_ => unit).
//        RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
//    }
//}

