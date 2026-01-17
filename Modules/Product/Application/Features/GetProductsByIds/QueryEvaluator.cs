using System.Runtime.CompilerServices;

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
                    options = options.AddInclude(p => p.ColorVariants);
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

    internal static string GetCacheKey(this GetProductsByIdsQuery query)
    {
        // Sort ProductIds to ensure consistent cache keys regardless of order
        var sortedIds = query.ProductIds
            .Select(id => id.Value.ToString())
            .OrderBy(id => id)
            .ToArray();

        var idsHash = string.Join(",", sortedIds);

        var handler = new DefaultInterpolatedStringHandler(20, 5);
        handler.AppendLiteral("products-by-ids:");
        handler.AppendFormatted(idsHash);
        handler.AppendLiteral(":");
        handler.AppendFormatted(query.PageNumber);
        handler.AppendLiteral(":");
        handler.AppendFormatted(query.PageSize);
        handler.AppendLiteral(":");
        handler.AppendFormatted(query.OrderBy ?? "");
        handler.AppendLiteral(":");
        handler.AppendFormatted(query.SortDir ?? "");
        handler.AppendLiteral(":");
        handler.AppendFormatted(query.Include ?? "");

        return handler.ToStringAndClear();
    }
}