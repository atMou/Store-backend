namespace Identity.Application.Events;

public record UserCreatedIntegrationEvent(string Name, string Email, string? VerificationCode, string? VerificationToken, DateTime? ExpiresAt) : IntegrationEvent
{

}