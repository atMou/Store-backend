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
            WithPagination = true,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize

        };

        if (!string.IsNullOrWhiteSpace(query.Brand))
            options = options.AddFilters(p => p.Brand.Name.Contains(query.Brand));

        if (!string.IsNullOrWhiteSpace(query.Category))
            options = options.AddFilters(p => p.Category.Name.Contains(query.Category));

        if (!string.IsNullOrWhiteSpace(query.Color))
            options = options.AddFilters(p => p.Colors.Any(c => c.Name.Contains(query.Color)));

        if (!string.IsNullOrWhiteSpace(query.Size))
            options = options.AddFilters(p => p.Sizes.Any(s => s.Name.Contains(query.Size)));

        if (!string.IsNullOrWhiteSpace(query.Search))
            options = options.AddFilters(p => p.Slug.Value.Contains(query.Search));

        if (query.MinPrice.HasValue)
            options = options.AddFilters(p => p.Price >= Money.FromDecimal(query.MinPrice));

        if (query.MaxPrice.HasValue)
            options = options.AddFilters(p => p.Price <= Money.FromDecimal(query.MaxPrice));

        if (query.IsNew.HasValue)
            options = options.AddFilters(p => p.Status.IsNew == query.IsNew);

        if (query.IsFeatured.HasValue)
            options = options.AddFilters(p => p.Status.IsFeatured == query.IsFeatured);

        if (query.IsTrending.HasValue)
            options = options.AddFilters(p => p.Status.IsTrending == query.IsTrending);

        if (query.IsBestSeller.HasValue)
            options = options.AddFilters(p => p.Status.IsBestSeller == query.IsBestSeller);



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
