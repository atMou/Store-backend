using Shared.Application.Contracts.Carts.Results;

namespace Basket.Application.Features.Cart.GetCart;

public record GetCartByUserIdQuery : IQuery<Fin<CartResult>>, IInclude
{
    public UserId UserId { get; init; }
    public string? Include { get; init; }
}

internal class GetCartByUserIdQueryHandler(BasketDbContext dbContext)
    : IQueryHandler<GetCartByUserIdQuery, Fin<CartResult>>
{
    public async Task<Fin<CartResult>> Handle(GetCartByUserIdQuery query, CancellationToken cancellationToken)
    {
        var loadCart =
            GetEntity<BasketDbContext, Domain.Models.Cart>(
                cart => cart.UserId == query.UserId,
                NotFoundError.New($"Cart not found for user {query.UserId.Value}."),
                opt => QueryEvaluator.Evaluate(opt, query)
                );

        var loadCoupons =
            GetEntities<BasketDbContext, Domain.Models.Coupon>(coupon => coupon.UserId == query.UserId);

        var db = (loadCoupons, loadCart).Apply((coupons, cart) => cart.ToResult(coupons));

        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}