
using Shared.Infrastructure.Clock;

namespace Shared.Persistence.Interceptors;

public class AuditableEntityInterceptor(IClock clock, IUserContext userContext) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntity(eventData);

        return base.SavingChanges(eventData, result);
    }


    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateAuditableEntity(eventData);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private Unit UpdateAuditableEntity(DbContextEventData eventData)
    {
        var user = Optional(userContext.GetCurrentUser<IO>().Run())
            .ToFin(UnAuthorizedError.New("You are not authorized to complete this action.")).ThrowIfFail();
        return Optional(eventData.Context).Map(cxt =>

             {
                 var auditableEntities = cxt.ChangeTracker.Entries<IEntity>();

                 foreach (EntityEntry<IEntity> entry in auditableEntities)
                 {
                     if (entry.State == EntityState.Added)
                     {
                         entry.Entity.CreatedAt = clock.UtcNow;
                         entry.Entity.CreatedBy = user.Name;
                     }
                     if (entry.State == EntityState.Modified || entry.HasModifiedOwnedEntity())
                     {
                         entry.Entity.UpdatedAt = clock.UtcNow;
                         entry.Entity.UpdatedBy = user.Name;
                     }
                 }

                 return unit;
             }



         ).Match(u => u, () => unit);
    }

}


public static class Extensions
{
    public static bool HasModifiedOwnedEntity(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State is EntityState.Added or EntityState.Modified);
}


