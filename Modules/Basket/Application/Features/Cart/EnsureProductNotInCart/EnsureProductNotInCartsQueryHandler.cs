namespace Basket.Application.Features.Cart.EnsureProductNotInCart;
internal class EnsureProductNotInCartsQueryHandler(BasketDbContext dbContext)
    : IQueryHandler<EnsureProductNotInCartsQuery, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(EnsureProductNotInCartsQuery request,
        CancellationToken cancellationToken)
    {
        return await Db<BasketDbContext>.liftIO(ctx => EnsureNotInCart(request.ProductId, ctx))
            .RunAsync(dbContext, EnvIO.New(null, cancellationToken));

    }

    private static IO<Unit> EnsureNotInCart(ProductId productId, BasketDbContext ctx)
    {
        return from exists in IO.liftAsync(e => ctx.Carts
                .Where(c => c.LineItems.Any(ci => ci.ProductId == productId))
                .AnyAsync(e.Token))
               from a in when(exists,
                   IO.fail<Unit>(NotFoundError.New($"Product with id '{productId}' is included some carts.")))
               select unit;
    }
}