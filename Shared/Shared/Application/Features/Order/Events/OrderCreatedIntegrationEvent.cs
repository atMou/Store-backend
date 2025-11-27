using Shared.Application.Abstractions;
using Shared.Application.Contracts.Order.Dtos;

namespace Shared.Application.Features.Order.Events;
public record OrderCreatedIntegrationEvent() : IntegrationEvent
{
    public OrderDto OrderDto { get; init; }
}

