using Shared.Application.Contracts;
using Shared.Application.Contracts.Carts.Results;

namespace Basket.Application.Features.Coupon.GetCouponByUserId;


public record GetCouponsByUserIdQuery(UserId UserId, int PageNumber = 1, int PageSize = 10) :
    IQuery<Fin<PaginatedResult<CouponResult>>>, IPagination;


internal class GetCouponByUserIdQueryHandler(BasketDbContext dbContext)
    : IQueryHandler<GetCouponsByUserIdQuery, Fin<PaginatedResult<CouponResult>>>
{
    public async Task<Fin<PaginatedResult<CouponResult>>> Handle(GetCouponsByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        var db = GetEntitiesWithPagination<BasketDbContext, Domain.Models.Coupon, CouponResult, GetCouponsByUserIdQuery>(
            c => c.UserId == query.UserId,
            options =>
        {
            options.AddPagination();
            options = options with
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                AsNoTracking = true,
            };
            return options;
        },
        query,
        coupon => coupon.ToResult());



        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
