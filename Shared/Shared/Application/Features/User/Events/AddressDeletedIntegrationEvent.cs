namespace Shared.Application.Features.User.Events;

public record AddressDeletedIntegrationEvent(Guid CartId, Guid OrderId, Address Address) : IntegrationEvent;

