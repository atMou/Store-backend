using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using Shared.Domain.Abstractions;

namespace Basket.Persistence.Extensions;
internal static class QueryExtensions
{

    public static IQueryable<TAggregate> WithOptions<TAggregate>(this IQueryable<TAggregate> queryable, Action<QueryableOptions<TAggregate>>? fn) where TAggregate : class, IAggregate
    {
        if (fn is not null)
        {
            var options = new QueryableOptions<TAggregate>();
            fn.Invoke(options);
            foreach (var include in options.Includes)
            {
                queryable = queryable.Include(include);
            }

            if (options.AsNoTracking)
            {
                queryable = queryable.AsNoTracking();
            }
            if (options.AsSplitQuery)
            {
                queryable = queryable.AsSplitQuery();
            }

        }

        return queryable;


    }
}

public record QueryableOptions<TAggregate>
{
    public bool AsSplitQuery { get; set; }
    public bool AsNoTracking { get; set; }

    internal List<Expression<Func<TAggregate, object>>> Includes { get; init; } = new();
    public QueryableOptions<TAggregate> AddInclude(Expression<Func<TAggregate, object>> include)
    {
        var includes = new List<Expression<Func<TAggregate, object>>>(Includes) { include };
        return this with { Includes = includes };
    }


}


