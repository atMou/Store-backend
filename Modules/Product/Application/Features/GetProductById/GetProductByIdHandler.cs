namespace Product.Application.Features.GetProductById;


internal class GetProductByIdQueryHandler(ProductDBContext dbContext)
    : IQueryHandler<GetProductByIdQuery, Fin<ProductResult>>
{
    public Task<Fin<ProductResult>> Handle(GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {


        var db = GetEntity<ProductDBContext, Domain.Models.Product>(
            product => product.Id == request.ProductId,
            opt =>
        {
            if (request.Include is not null && request.Include.Any())
            {
                opt.AsNoTracking = true;
                opt.AsSplitQuery = true;
                opt.AddInclude(request.Include);
            }
            return opt;
        }, NotFoundError.New($"Product with ID {request.ProductId} not found")).Map(p => p.ToResult());

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));

    }


}