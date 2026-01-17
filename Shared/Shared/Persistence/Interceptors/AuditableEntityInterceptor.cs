using Shared.Infrastructure.Clock;

namespace Shared.Persistence.Interceptors;

public class AuditableEntityInterceptor(IClock clock, IUserContext userContext) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntity(eventData);

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        await UpdateAuditableEntityAsync(eventData, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private Unit UpdateAuditableEntity(DbContextEventData eventData)
    {
        var user =
            userContext.GetCurrentUserF<Fin>().As()
                .IfFail(new CurrentUser(Guid.Empty, "System", "System", false));

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

    private async ValueTask<Unit> UpdateAuditableEntityAsync(DbContextEventData eventData, CancellationToken cancellationToken)
    {
        var user =
            userContext.GetCurrentUserF<Fin>().As()
                .IfFail(new CurrentUser(Guid.Empty, "System", "System", false));

        return await Task.Run(() =>
        {
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
            }).Match(u => u, () => unit);
        }, cancellationToken);
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