namespace Shared.Domain.Abstractions;

public record Aggregate<TId>(TId Id) : Entity<TId>(Id), IAggregate<TId> where TId : IId
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public IDomainEvent[] ClearDomainEvents()
    {
        var events = _domainEvents.ToArray();
        _domainEvents.Clear();
        return events;
    }

    protected LanguageExt.Unit AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
        return Unit.Default;
    }
}