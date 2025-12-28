using Shared.Application.Abstractions;

namespace Shared.Application.Features.User.Events;

public record UserEmailVerifiedIntegrationEvent(Guid UserId, Address Address) : IntegrationEvent
{
}
