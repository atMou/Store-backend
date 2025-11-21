
namespace Product.Application.Features.GetProductById;


internal class GetProductByIdQueryHandler(ProductDBContext dbContext, IProductRepository productRepository)
    : IQueryHandler<GetProductByIdQuery, Fin<ProductDto>>
{
    public Task<Fin<ProductDto>> Handle(GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var db = from res in Db<ProductDBContext>.liftIO(ctx =>
                productRepository.GetProductById(request.ProductId, ctx, opt =>
                {
                    if (request.Include is not null && request.Include.Any())
                    {
                        opt.AsNoTracking = true;
                        opt.AsSplitQuery = true;
                        opt.AddInclude(request.Include);
                    }
                }))
                 select res.ToDto();

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));

    }


}