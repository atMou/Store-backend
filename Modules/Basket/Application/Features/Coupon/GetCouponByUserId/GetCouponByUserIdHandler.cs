using Basket.Application.Contracts;

using Shared.Application.Contracts;
using Shared.Application.Contracts.Carts.Results;

namespace Basket.Application.Features.Coupon.GetCouponByUserId;


public record GetCouponsByUserIdQuery(
    UserId UserId,
    int PageNumber = 1,
    int PageSize = 10,
    string? Status = null)
    : IQuery<Fin<PaginatedResult<CouponResult>>>, IPagination;


internal class GetCouponByUserIdQueryHandler(BasketDbContext dbContext)
    : IQueryHandler<GetCouponsByUserIdQuery, Fin<PaginatedResult<CouponResult>>>
{
    public async Task<Fin<PaginatedResult<CouponResult>>> Handle(
        GetCouponsByUserIdQuery query,
        CancellationToken cancellationToken)
    {


        var db =

            GetEntitiesWithPagination<BasketDbContext, Domain.Models.Coupon, CouponResult, GetCouponsByUserIdQuery>(
            coupon => coupon.UserId == query.UserId,
            options => QueryEvaluator.Evaluate(options, query),
            query,
            coupon => coupon.ToResult());

        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
