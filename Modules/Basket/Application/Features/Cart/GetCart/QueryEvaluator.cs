using Shared.Persistence.Extensions;

namespace Basket.Application.Features.Cart.GetCart;
internal static class QueryEvaluator
{
    public static QueryOptions<Domain.Models.Cart> Evaluate<TInclude>(QueryOptions<Domain.Models.Cart> options, TInclude query) where TInclude : IInclude
    {
        options = options with
        {
            AsNoTracking = true,

        };

        if (!string.IsNullOrEmpty(query.Include))
        {
            var includes = query.Include.Split(',');
            options = options with { AsSplitQuery = true };

            foreach (string se in includes.Distinct())
            {
                if (string.Equals(se, "lineItems", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.LineItems);
                }
                if (string.Equals(se, "couponIds", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.CouponIds);
                }

            }
        }

        return options;
    }


}


