using Shared.Application.Contracts.Carts.Results;

namespace Basket.Application.Features.Cart.GetCart;



public record GetCartByCartIdQuery : IQuery<Fin<CartResult>>, IInclude
{
    public CartId CartId { get; init; }
    public string? Include { get; init; }
}

internal class GetCartByCartIdQueryHandler(
    BasketDbContext dbContext)
    : IQueryHandler<GetCartByCartIdQuery, Fin<CartResult>>
{
    public Task<Fin<CartResult>> Handle(GetCartByCartIdQuery query, CancellationToken cancellationToken)
    {

        var loadCart =
        GetEntity<BasketDbContext, Domain.Models.Cart>(
            cart => cart.Id == query.CartId,
            opt => CartQueryEvaluator.Evaluate(opt, query),
            NotFoundError.New($"Cart not found for id {query.CartId.Value}."));

        var loadCoupons =
            GetEntities<BasketDbContext, Domain.Models.Coupon>(coupon => coupon.CartId == query.CartId);

        var db = (loadCoupons, loadCart).Apply((coupons, cart) => cart.ToResult(coupons));

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}