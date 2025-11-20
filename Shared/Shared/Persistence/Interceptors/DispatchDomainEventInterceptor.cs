namespace Shared.Persistence.Interceptors;

public class DispatchDomainEventInterceptor(IMediator mediator) : SaveChangesInterceptor
{


    //public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    //{
    //    DispatchDomainEvents(eventData);
    //    return base.SavingChanges(eventData, result);
    //}

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await DispatchDomainEvents(eventData);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }



    private async Task DispatchDomainEvents(DbContextEventData eventData)
    {

        var entries = eventData.Context?.ChangeTracker.Entries<IAggregate>() ?? [];
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


    }

}