
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
                        OrderQueryEvaluator(options, query))
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


    private QueryOptions<Domain.Models.Order> OrderQueryEvaluator(QueryOptions<Domain.Models.Order> options, GetOrdersQuery query)
    {

        options = options with
        {
            AsNoTracking = true,
            AsSplitQuery = true,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        if (!string.IsNullOrWhiteSpace(query.Email))
            options = options.AddFilters(o => o.Email.Value.Contains(query.Email));


        if (!string.IsNullOrWhiteSpace(query.OrderStatus))
            options = options.AddFilters(o => o.OrderStatus.Name.Contains(query.OrderStatus));

        if (query.MinTotal.HasValue)
            options = options.AddFilters(o => o.Total.Value >= query.MinTotal.Value);

        if (query.MaxTotal.HasValue)
            options = options.AddFilters(o => o.Total.Value <= query.MaxTotal.Value);

        if (query.Include is { Length: > 0 })
        {
            foreach (string se in query.Include)
            {
                if (string.Equals(se, "orderItems", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.OrderItems);
                }

            }
        }

        if (!string.IsNullOrWhiteSpace(query.OrderBy))
        {
            var sortBy = query.OrderBy.ToLowerInvariant();

            options = sortBy switch
            {
                "total" => options.AddOrderBy(p => p.Total.Value),
                _ => options
            };
        }

        if (!string.IsNullOrWhiteSpace(query.SortDir))
        {
            var sortDir = query.SortDir.ToLowerInvariant();
            options = sortDir switch
            {
                "asc" => options.AddSortDirAsc(),
                "desc" => options.AddSortDirDesc(),
                _ => options
            };
        }

        return options;
    }

}

