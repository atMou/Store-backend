namespace Shared.Domain.Abstractions;

public interface IAggregate<TId> : IAggregate, IEntity<TId> where TId : IId;

public interface IAggregate : IEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    IDomainEvent[] ClearDomainEvents();
}
