using Shared.Application.Contracts.Carts.Results;

namespace Basket.Application.Features.Cart.GetCart;

public record GetCartByCartIdQuery : IQuery<Fin<CartResult>>, IInclude
{
    public CartId CartId { get; init; }
    public string? Include { get; init; }
}

internal class GetCartByCartIdQueryHandler(
    IUserContext userContext,
    BasketDbContext dbContext,
    ISender sender)
    : IQueryHandler<GetCartByCartIdQuery, Fin<CartResult>>
{
    public async Task<Fin<CartResult>> Handle(GetCartByCartIdQuery query, CancellationToken cancellationToken)
    {
        var loadCart =
            GetEntity<BasketDbContext, Domain.Models.Cart>(
                cart => cart.Id == query.CartId,
                NotFoundError.New($"Cart not found for id {query.CartId.Value}."),
                opt => QueryEvaluator.Evaluate(opt, query)
            );

        var loadCoupons =
            GetEntities<BasketDbContext, Domain.Models.Coupon>(coupon => coupon.CartId == query.CartId);


        var result = (loadCoupons, loadCart).Apply((coupons, cart) => (coupons, cart));


        var db = from res in result
                 from _ in EnsureHasPermission(res.cart.UserId)
                 select res.cart.ToResult(res.coupons);

        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private Fin<Unit> EnsureHasPermission(UserId userId) =>
        userContext.IsSameUserF<Fin>(userId, UnAuthorizedError.New("You have no permission to get the cart")).As()
        | userContext.HasRoleF<Fin>(Role.Admin,
            UnAuthorizedError.New("You have no permission to get the cart"))
        | userContext.HasRoleF<Fin>(Role.Support,
            UnAuthorizedError.New("You have no permission to get the cart"));
}