namespace Product.Application.Features.GetProductsByIds;

public static class QueryEvaluator
{
    public static QueryOptions<Domain.Models.Product> Evaluate(QueryOptions<Domain.Models.Product> options,
        GetProductsByIdsQuery query)
    {

        options = options with
        {
            AsNoTracking = true,
            AsSplitQuery = true,
            WithPagination = true,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize

        };

        if (!string.IsNullOrEmpty(query.Include))
        {
            var includes = query.Include.Split(',').Select(s => s.Trim());

            foreach (string se in includes.Distinct())
            {
                if (string.Equals(se, "variants", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Alternatives);
                }

                if (string.Equals(se, "reviews", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Reviews);
                }

                if (string.Equals(se, "images", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Images);
                }

            }
        }

        if (!string.IsNullOrWhiteSpace(query.OrderBy))
        {
            var sortBy = query.OrderBy.ToLowerInvariant();

            options = sortBy switch
            {

                "price" => options.AddOrderBy(p => p.Price.Value),
                "brand" => options.AddOrderBy(p => p.Brand.Name),
                "totalsales" => options.AddOrderBy(p => p.TotalSales),
                "totalreviews" => options.AddOrderBy(p => p.TotalReviews),
                "averagerating" => options.AddOrderBy(p => p.AverageRating),
                _ => options
            };
        }
        else
        {
            options = options.AddOrderBy(p => p.Id);
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
