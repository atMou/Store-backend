namespace Shared.Persistence.Extensions;


public static class GlobalFilterExtensions
{

    public static ModelBuilder ApplyGlobalFilterButIgnoreIfHasPermission<TEntity>(
        this ModelBuilder modelBuilder,
        IEnumerable<Permission> permissions,
        Expression<Func<TEntity, bool>> filter,
        IUserContext userContext)
        where TEntity : class
    {
        var hasPermission = permissions.AsIterable()
            .Traverse(permission => userContext.HasPermissionF<Fin>(permission, Error.Empty)).As().IsSucc;


        if (hasPermission)
        {
            return modelBuilder;
        }

        modelBuilder.Entity<TEntity>().HasQueryFilter(filter);
        return modelBuilder;
    }
}
