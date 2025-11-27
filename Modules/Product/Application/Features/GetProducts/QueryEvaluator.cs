namespace Product.Application.Features.GetProducts;

public static class QueryEvaluator
{
    public static QueryOptions<Domain.Models.Product> Evaluate(QueryOptions<Domain.Models.Product> options,
    GetProductsQuery query)
    {

        options = options with
        {
            AsNoTracking = true,
            AsSplitQuery = true,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize

        };

        if (!string.IsNullOrWhiteSpace(query.Brand))
            options = options.AddFilters(p => p.Brand.Name.Contains(query.Brand));


        if (!string.IsNullOrWhiteSpace(query.Category))
            options = options.AddFilters(p => p.Category.Name.Contains(query.Category));

        if (!string.IsNullOrWhiteSpace(query.Color))
            options = options.AddFilters(p => p.Color.Name.Contains(query.Color));

        if (!string.IsNullOrWhiteSpace(query.Size))
            options = options.AddFilters(p => p.Size.Name.Contains(query.Size));

        if (!string.IsNullOrWhiteSpace(query.Search))
            options = options.AddFilters(p => p.Slug.Value.Contains(query.Search));

        if (query.MinPrice.HasValue)
            options = options.AddFilters(p => p.Price.Value >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            options = options.AddFilters(p => p.Price.Value <= query.MaxPrice.Value);

        if (!string.IsNullOrEmpty(query.Include))
        {
            var includes = query.Include.Split(',');

            foreach (string se in includes.Distinct())
            {
                if (string.Equals(se, "variants", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Variants);
                }

                if (string.Equals(se, "reviews", StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Reviews);
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
