using System.Linq.Expressions;

using Shared.Persistence.Extensions;

namespace Basket.Application.Features.Coupon.GetCouponByUserId;


public static class QueryEvaluator
{
    public static QueryOptions<Domain.Models.Coupon> Evaluate(
        QueryOptions<Domain.Models.Coupon> options,
        GetCouponsByUserIdQuery query)
    {
        options = options.AddPagination();
        options = options with
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            AsNoTracking = true,
        };

        if (query.Status is { } s)
        {
            var statuses = s.Split(',').Select(s1 => CouponStatus.FromUnsafe(s1));
            var statusFilters = statuses
                .Select(status => (Expression<Func<Domain.Models.Coupon, bool>>)(c => c.CouponStatus == status))
                .ToArray();

            if (statusFilters.Length > 0)
            {
                options = options.AddOrFilters(statusFilters);
            }
        }
        return options;
    }



}

