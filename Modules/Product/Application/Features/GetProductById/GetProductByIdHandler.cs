
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
                    opt.AsNoTracking = true;
                    opt.AsSplitQuery = true;
                    if (request.IncludeRelated)
                    {
                        opt.AddInclude(p => p.ProductImages);
                        opt.AddInclude(p => p.Reviews);
                        opt.AddInclude(p => p.Variants);
                    }
                }))
                 select res.ToDto();

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));

    }


}