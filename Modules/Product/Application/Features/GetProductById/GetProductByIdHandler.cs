namespace Product.Application.Features.GetProductById;

internal class GetProductByIdQueryHandler(ProductDBContext dbContext)
    : IQueryHandler<GetProductByIdQuery, Fin<ProductResult>>
{

    public async Task<Fin<ProductResult>> Handle(GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {

        var db = GetEntity<ProductDBContext, Domain.Models.Product>(
            product => product.Id == query.ProductId
            , NotFoundError.New($"Product with ID {query.ProductId} not found"),
            opt => QueryEvaluator.Evaluate(opt, query)
            )
            .Map(p => p.ToResult());

        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));

    }
}