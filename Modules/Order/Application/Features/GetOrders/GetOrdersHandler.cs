
using Order.Persistence;

using Shared.Application.Contracts;
using Shared.Application.Contracts.Order.Results;

namespace Order.Application.Features.GetOrders;

public class GetOrdersQuery : IQuery<Fin<PaginatedResult<OrderResult>>>
{
    public Guid UserId { get; init; }
    public string? Email { get; init; }
    public decimal? Subtotal { get; init; }
    public decimal? Total { get; init; }
    public string? OrderStatus { get; init; }
    public decimal? MinTotal { get; init; }
    public decimal? MaxTotal { get; init; }
    public string? OrderBy { get; init; }
    public string? SortDir { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string[]? Include { get; init; } = [];
}

internal class GetOrdersQueryHandler(OrderDBContext dbContext)
    : IQueryHandler<GetOrdersQuery, Fin<PaginatedResult<OrderResult>>>
{
    public Task<Fin<PaginatedResult<OrderResult>>> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        var db =
            from res in Db<OrderDBContext>.liftIO(async (ctx, e) =>
                await ctx.Orders.WithQueryOptions(options =>
                        QueryEvaluator.Evaluate(options, query))
                .GroupBy(_ => 1)
                .Select(g => new { TotalCount = g.Count(), Items = g.ToList() })
                .FirstOrDefaultAsync(e.Token)
                .Map(res => (res!.Items, res.TotalCount)))

            select new PaginatedResult<OrderResult>()
            {
                Items = res.Items.ToResult(),
                PageSize = query.PageSize,
                PageNumber = query.PageNumber,
                TotalCount = res.TotalCount
            };

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}

