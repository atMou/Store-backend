using Product.Application.Features.GetProducts;

namespace Product.Persistence.Extensions;
internal static class QueryExtensions
{
    public static IQueryable<Domain.Models.Product> GetQueryable(this IQueryable<Domain.Models.Product> queryable, GetProductQuery query)
    {
        if (query.Category != null)
        {
            queryable = queryable.Where(p => p.Category.Name == query.Category);
        }

        if (query.Brand != null)
        {
            queryable = queryable.Where(p => p.Brand.Name == query.Brand);
        }

        if (query.Color != null)
        {
            queryable = queryable.Where(p => p.Color.Name == query.Color);
        }

        if (query.MinPrice != null)
        {
            queryable = queryable.Where(p => p.Price.Value >= query.MinPrice);
        }

        if (query.MaxPrice != null)
        {
            queryable = queryable.Where(p => p.Price.Value <= query.MaxPrice);
        }

        if (query.Search != null)
        {
            queryable = queryable.Where(p => p.Slug.Value.Contains(query.Search));
        }

        if (query.includeReviews != null)
        {
            queryable.Include(p => p.Reviews).AsSplitQuery();
        }
        if (query.includeVariants != null)
        {
            queryable.Include(p => p.Variants).AsSplitQuery(); ;
        }

        if (query.SortBy != null)
        {
            queryable = query.SortDir == "asc"
                ? queryable.OrderBy(p => EF.Property<object>(p, query.SortBy))
                : queryable.OrderByDescending(p => EF.Property<object>(p, query.SortBy));
        }

        return queryable.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).AsNoTracking();
    }
}
