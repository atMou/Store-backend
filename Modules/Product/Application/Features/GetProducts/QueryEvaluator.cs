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
        {
            var brands = query.Brand.Split(',')
                .Select(b => b.Trim())
                .Where(b => !string.IsNullOrEmpty(b)).ToList();
            var bs = brands.Any() ? Brand.Like(brands).ToList() : [];
            if (bs.Any())
            {
                options = options.AddFilters(p => bs.Contains(p.Brand));
            }

        }


        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var main = query.Category.Trim();
            options = options.AddFilters(p => p.Category.Main == main);
        }

        if (!string.IsNullOrWhiteSpace(query.SubCategory))
        {
            var subs = query.SubCategory.Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s)).ToList();
            var matches = subs.Any() ? Category.SubLiKe(subs).ToList() : [];

            if (matches.Any())
            {
                options = options.AddFilters(p => matches.Contains(p.Category.Sub));
            }
        }

        if (!string.IsNullOrWhiteSpace(query.Color))
        {
            var colors = query.Color.Split(',')
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrEmpty(c)).ToArray();

            var cs = colors.Any() ? Color.Like(colors).ToList() : [];
            if (colors.Length > 0)
            {
                options = options.AddFilters(p => p.Variants.Any(v => cs.Contains(v.Color)));
            }

        }

        if (!string.IsNullOrWhiteSpace(query.Size))
        {
            var sizes = query.Size.Split(',')
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrEmpty(c)).ToArray();

            var ss = sizes.Any() ? Size.Like(sizes).ToList() : [];
            if (sizes.Length > 0)
            {
                options = options.AddFilters(p => p.Variants.Any(v => ss.Contains(v.Size)));


            }

        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            options = options.AddFilters(p => EF.Functions.Like(p.Slug.Value, $"%{search}%"));
        }


        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            options = options.AddFilters(p => EF.Functions.Like(p.ProductType.Type, $"%{query.Type}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.Sub))
        {
            options = options.AddFilters(p => EF.Functions.Like(p.ProductType.SubType, $"%{query.Sub}%"));
        }

        if (query.MinPrice.HasValue)
            options = options.AddFilters(p => EF.Property<decimal>(p, "Price") >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            options = options.AddFilters(p => EF.Property<decimal>(p, "Price") <= query.MaxPrice.Value);

        if (query.IsNew.HasValue)
            options = options.AddFilters(p => p.Status.IsNew == query.IsNew.Value);

        if (query.IsFeatured.HasValue)
            options = options.AddFilters(p => p.Status.IsFeatured == query.IsFeatured.Value);

        if (query.IsTrending.HasValue)
            options = options.AddFilters(p => p.Status.IsTrending == query.IsTrending.Value);

        if (query.IsBestSeller.HasValue)
            options = options.AddFilters(p => p.Status.IsBestSeller == query.IsBestSeller.Value);



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
