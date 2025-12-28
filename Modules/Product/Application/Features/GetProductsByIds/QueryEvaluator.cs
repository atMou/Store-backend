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
            var includes = query.Include.Split(',');

            foreach (string se in includes.Distinct())
            {
                if (string.Equals(se, Alternatives, StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Alternatives);
                }

                if (string.Equals(se, Reviews, StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Reviews);
                }


                if (string.Equals(se, Images, StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Images);
                }


                if (string.Equals(se, Variants, StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.Variants);
                }

                if (string.Equals(se, MaterialDetails, StringComparison.OrdinalIgnoreCase))
                {
                    options = options.AddInclude(p => p.MaterialDetails);
                }


            }
        }

        if (!string.IsNullOrWhiteSpace(query.OrderBy))
        {
            var sortBy = query.OrderBy.ToLowerInvariant();

            options = sortBy switch
            {

                "price" => options.AddOrderBy(p => EF.Property<decimal>(p, "Price")),
                "brand" => options.AddOrderBy(p => p.Brand),
                "totalsales" => options.AddOrderBy(p => p.TotalSales),
                "totalreviews" => options.AddOrderBy(p => p.TotalReviews),
                "averagerating" => options.AddOrderBy(p => p.AverageRating),
                _ => options.AddOrderBy(p => p.Id)
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
