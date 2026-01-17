namespace Order.Application.Features.GetOrders;

public static class QueryEvaluator
{

    public static QueryOptions<Domain.Models.Order> Evaluate(QueryOptions<Domain.Models.Order> options, GetOrdersQuery query)
    {

        options = options with
        {
            AsNoTracking = true,
            AsSplitQuery = true,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        if (!string.IsNullOrWhiteSpace(query.Email))
            options = options.AddFilters(o => o.Email.Contains(query.Email));


        if (!string.IsNullOrWhiteSpace(query.OrderStatus))
            options = options.AddFilters(o => o.OrderStatus.Name.Contains(query.OrderStatus));

        if (query.MinTotal.HasValue)
            options = options.AddFilters(o => o.Total >= query.MinTotal.Value);

        if (query.MaxTotal.HasValue)
            options = options.AddFilters(o => o.Total <= query.MaxTotal.Value);

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
                "total" => options.AddOrderBy(p => p.Total),
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
