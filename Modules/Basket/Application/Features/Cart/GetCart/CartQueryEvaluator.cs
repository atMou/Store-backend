using Shared.Persistence.Extensions;

namespace Basket.Application.Features.Cart.GetCart;
internal static class CartQueryEvaluator
{
    public static QueryOptions<Domain.Models.Cart> Evaluate(QueryOptions<Domain.Models.Cart> options, ICartQuery query)
    {
        options = options with
        {
            AsNoTracking = true,

        };

        if (query.Include is { Length: > 0 })
        {
            options = options with { AsSplitQuery = true };

            foreach (string se in query.Include)
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

interface ICartQuery
{
    public string[]? Include { get; init; }
}
