using Microsoft.EntityFrameworkCore.Metadata;

using Shared.Domain.Filters;

namespace Shared.Persistence.Extensions;


public static class GlobalFilterExtensions
{

    public static ModelBuilder ApplyGlobalFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                ApplySoftDeleteFilter(modelBuilder, entityType);
            }


        }

        return modelBuilder;
    }

    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder, IMutableEntityType entityType)
    {
        var parameter = Expression.Parameter(entityType.ClrType, "e");
        var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
        var notDeleted = Expression.Not(property);
        var lambda = Expression.Lambda(notDeleted, parameter);

        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
    }

    public static ModelBuilder ApplyGlobalFilter<TEntity>(
        this ModelBuilder modelBuilder,
        Expression<Func<TEntity, bool>> filter)
        where TEntity : class
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(filter);
        return modelBuilder;
    }

    public static Expression<Func<T, bool>> CombineFilters<T>(
        params Expression<Func<T, bool>>[] filters)
    {
        if (filters.Length == 0)
            return x => true;

        if (filters.Length == 1)
            return filters[0];

        var parameter = Expression.Parameter(typeof(T), "x");

        Expression? combined = null;
        foreach (var filter in filters)
        {
            var invoked = Expression.Invoke(filter, parameter);
            combined = combined == null ? invoked : Expression.AndAlso(combined, invoked);
        }

        return Expression.Lambda<Func<T, bool>>(combined!, parameter);
    }
}
