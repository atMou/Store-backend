using Shared.Application.Contracts.Carts.Results;
using Shared.Presentation;

namespace Basket.Application.Features.Coupon.GetCouponByUserId;


public record GetCouponsByUserIdQuery(UserId UserId, int PageNumber = 1, int PageSize = 10) :
    IQuery<Fin<PaginatedResult<CouponResult>>>, IPagination;


internal class GetCouponByUserIdQueryHandler(BasketDbContext dbContext)
    : IQueryHandler<GetCouponsByUserIdQuery, Fin<PaginatedResult<CouponResult>>>
{
    public Task<Fin<PaginatedResult<CouponResult>>> Handle(GetCouponsByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        var db = GetEntitiesWithPagination<BasketDbContext,
            Domain.Models.Coupon,
            CouponResult,
            GetCouponsByUserIdQuery>(
            query,
            coupon => coupon.ToResult(),
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
        });




        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
