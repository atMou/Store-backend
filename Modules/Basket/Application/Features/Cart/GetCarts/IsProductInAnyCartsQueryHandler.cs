namespace Basket.Application.Features.Cart.GetCarts;
internal class IsProductInAnyCartsQueryHandler(ICartRepository cartRepository, BasketDbContext dbContext)
    : IQueryHandler<IsProductInAnyCartsQuery, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(IsProductInAnyCartsQuery request,
        CancellationToken cancellationToken)
    {
        return Db<BasketDbContext>.liftIO(ctx => cartRepository.ProductIncludedInAnyCart(request.ProductId, ctx))
            .RunAsync(dbContext, EnvIO.New(null, cancellationToken));

    }
}