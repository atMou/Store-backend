using System.Linq.Expressions;

namespace Shared.Persistence.Extensions;

public static class QueryableExtensions
{

    public static IQueryable<TAggregate> WithQueryOptions<TAggregate>(this IQueryable<TAggregate> queryable,
        Func<QueryOptions<TAggregate>, QueryOptions<TAggregate>>? fn)
        where TAggregate : class, IAggregate
    {
        if (fn is not null)
        {
            var _options = new QueryOptions<TAggregate>();
            var options = fn.Invoke(_options);
            foreach (var include in options.IncludeExpressions)
            {
                queryable = queryable.Include(include);
            }

            //foreach (string s in options.Includes)
            //{
            //    queryable = s switch
            //    {
            //        var _ when string.Equals("variants", s, StringComparison.InvariantCultureIgnoreCase) => queryable
            //            .Include("Variants"),
            //        var _ when string.Equals("images", s, StringComparison.InvariantCultureIgnoreCase) => queryable
            //            .Include("ProductImages"),
            //        var _ when string.Equals("reviews", s, StringComparison.InvariantCultureIgnoreCase) => queryable
            //            .Include("Reviews"),
            //        _ => queryable
            //    };
            //}
            foreach (var filter in options.FilterExpressions)
            {
                queryable = queryable.Where(filter);
            }

            if (options.AsNoTracking)
            {
                queryable = queryable.AsNoTracking();
            }
            if (options.AsSplitQuery)
            {
                queryable = queryable.AsSplitQuery();
            }
            if (options is { OrderAsc: true, OrderByExpression: not null })
            {
                queryable = queryable.OrderBy(options.OrderByExpression!);
            }
            else if (options is { OrderDesc: true, OrderByExpression: not null })
            {
                queryable = queryable.OrderByDescending(options.OrderByExpression!);
            }
            if (!options.WithPagination) return queryable;

            var skip = (options.PageNumber - 1) * options.PageSize;
            queryable = queryable.Skip(skip).Take(options.PageSize);

        }

        return queryable;

    }

}


public record QueryOptions<TAggregate>
{
    public string[] Includes { get; set; } = [];
    public bool AsSplitQuery { get; set; }
    public bool AsNoTracking { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public bool WithPagination { get; set; }

    public bool OrderAsc { get; set; }
    public bool OrderDesc { get; set; }

    internal List<Expression<Func<TAggregate, object>>> IncludeExpressions { get; private init; } = [];
    internal List<Expression<Func<TAggregate, bool>>> FilterExpressions { get; private init; } = [];

    public QueryOptions<TAggregate> AddInclude(params Expression<Func<TAggregate, object>>[] includes)
    {
        List<Expression<Func<TAggregate, object>>> includesList = [.. IncludeExpressions, .. includes];
        return this with { IncludeExpressions = includesList };
    }
    public QueryOptions<TAggregate> AddInclude(Expression<Func<TAggregate, object>> include)
    {
        List<Expression<Func<TAggregate, object>>> includes = [.. IncludeExpressions, include];
        return this with { IncludeExpressions = includes };
    }

    public QueryOptions<TAggregate> AddInclude(params string[] includeParams)
    {
        return this with { Includes = [.. Includes, .. includeParams] };
    }

    public QueryOptions<TAggregate> AddFilters(params Expression<Func<TAggregate, bool>>[] _filters)
    {
        List<Expression<Func<TAggregate, bool>>> filters = [.. FilterExpressions, .. _filters];
        return this with { FilterExpressions = filters };
    }

    public Expression<Func<TAggregate, object>>? OrderByExpression { get; private set; }


    public QueryOptions<TAggregate> AddOrderBy(Expression<Func<TAggregate, object>> orderBy)
    {
        return this with { OrderByExpression = orderBy };
    }

    public QueryOptions<TAggregate> AddSortDirAsc()
    {
        return this with { OrderAsc = true, OrderDesc = false };
    }
    public QueryOptions<TAggregate> AddSortDirDesc()
    {
        return this with { OrderAsc = false, OrderDesc = true };
    }

    public QueryOptions<TAggregate> AddPagination()
    {
        return this with { WithPagination = true };
    }

}
