namespace Shared.Persistence.Interceptors;

public class DispatchDomainEventInterceptor(IMediator mediator) : SaveChangesInterceptor
{
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        DispatchDomainEvents(eventData);
        return base.SavedChanges(eventData, result);
    }


    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        DispatchDomainEvents(eventData);
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private Unit DispatchDomainEvents(DbContextEventData eventData)
    {
        var option = Optional(eventData.Context).Map(async ctx =>

        {
            var entries = ctx.ChangeTracker.Entries<IAggregate>();
            foreach (var entry in entries)
            {
                var aggregate = entry.Entity;
                var domainEvents = aggregate.DomainEvents.ToList();
                aggregate.ClearDomainEvents();

                foreach (var domainEvent in domainEvents)
                {
                    await mediator.Publish(domainEvent);
                }
            }
        });
        return unit;
    }

}