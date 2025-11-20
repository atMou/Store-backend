using Shared.Infrastructure.Clock;

namespace Shared.Persistence.Interceptors;

public class AuditableEntityInterceptor(IClock clock, IUserContext userContext) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntity(eventData);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        UpdateAuditableEntity(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private Unit UpdateAuditableEntity(DbContextEventData eventData)
    {
        var user =
            Optional(userContext.GetCurrentUser<IO>().As().IfFail(new CurrentUser(Guid.Empty, "System", "System"))
                .Run()).ValueUnsafe();

        return Optional(eventData.Context).Map(cxt =>

            {
                var auditableEntities = cxt.ChangeTracker.Entries<IEntity>();

                foreach (var entry in auditableEntities)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.CreatedAt = clock.UtcNow;
                        entry.Entity.CreatedBy = user!.Email;
                    }

                    if (entry.State == EntityState.Modified || entry.HasModifiedOwnedEntity())
                    {
                        entry.Entity.UpdatedAt = clock.UtcNow;
                        entry.Entity.UpdatedBy = user!.Email;
                    }
                }

                return unit;
            }
        ).Match(u => u, () => unit);
    }
}

public static class Extensions
{
    public static bool HasModifiedOwnedEntity(this EntityEntry entry)
    {
        return entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State is EntityState.Modified);
    }
}