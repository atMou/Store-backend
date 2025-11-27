using Shared.Application.Contracts.Order.Dtos;

namespace Order.Domain.Events;

public record OrderCreatedDomainEvent : IDomainEvent
{
    public OrderDto OrderDto { get; init; }

}