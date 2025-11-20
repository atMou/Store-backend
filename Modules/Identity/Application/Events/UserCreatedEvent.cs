namespace Identity.Application.Events;

public record UserCreatedEvent(string Email, Guid? VerificationToken, DateTime? ExpiresAt) : IDomainEvent
{
}
