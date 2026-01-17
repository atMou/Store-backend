namespace Product.Application.Features.AddProductAlternatives;
public record AddProductAlternativesCommand : ICommand<Fin<Unit>>
{

    public ProductId ProductId { get; init; }

    public IEnumerable<ProductId> AlternativeProductIds { get; init; } = [];
}
internal class AddProductAlternativesCommandHandler(ProductDBContext dbContext)
    : ICommandHandler<AddProductAlternativesCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(AddProductAlternativesCommand command, CancellationToken cancellationToken)
    {
        var db =
            from alternativeProducts in GetEntities<ProductDBContext, Domain.Models.Product>(
                p => command.AlternativeProductIds.Contains(p.Id))

            from _ in GetUpdateEntity<ProductDBContext, Domain.Models.Product>(
                p => p.Id == command.ProductId,
                NotFoundError.New($"Product with ID '{command.ProductId.Value}' was not found."),
                opt =>
                {
                    opt.AsSplitQuery = true;
                    opt = opt.AddInclude(p => p.Alternatives);
                    return opt;
                },
                product => product.AddAlternatives([.. alternativeProducts]))

            select unit;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
