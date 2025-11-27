using Shared.Application.Contracts.Order.Results;

namespace Order.Application.Features.GetOrdersByUserId;

public class GetOrdersByUserIdQuery : IQuery<Fin<PaginatedResult<OrderResult>>>, IPagination, IInclude
{
    public UserId UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Include { get; init; }
}

internal class GetOrderByIdCommandHandler(OrderDBContext dbContext)
    : IQueryHandler<GetOrdersByUserIdQuery, Fin<PaginatedResult<OrderResult>>>
{
    public Task<Fin<PaginatedResult<OrderResult>>> Handle(GetOrdersByUserIdQuery query, CancellationToken cancellationToken)
    {
        var db =
            from res in GetEntitiesWithPagination<OrderDBContext, Domain.Models.Order, OrderResult,
                GetOrdersByUserIdQuery>(
                order => order.UserId == query.UserId,
                query,
                o => o.ToResult(),
                opt => QueryEvaluator(opt, query)
            )

            select res;

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private QueryOptions<Domain.Models.Order> QueryEvaluator(QueryOptions<Domain.Models.Order> options, GetOrdersByUserIdQuery query)
    {
        options.AsNoTracking = true;
        options.AddPagination();
        options.PageSize = query.PageSize;
        options.PageNumber = query.PageNumber;

        if (!string.IsNullOrEmpty(query.Include))
        {
            options.AsSplitQuery = true;

            var includes = query.Include.Split(',');

            foreach (string se in includes.Distinct())
            {
                if (string.Equals(se, "lineItems", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(o => o.OrderItems);
                }

                if (string.Equals(se, "couponIds", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(o => o.CouponIds);
                }

            }
        }

        return options;
    }
}

