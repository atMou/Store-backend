namespace Identity.Application.EventHandlers;

public record UserCreatedIntegrationEvent(string Email, Guid? VerificationToken, DateTime? ExpiresAt) : IntegrationEvent
{

}