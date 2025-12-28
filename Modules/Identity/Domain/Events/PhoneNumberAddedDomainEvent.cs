namespace Identity.Domain.Events;

public record PhoneNumberAddedDomainEvent(Phone Phone, string ConfirmationToken) : IDomainEvent
{

}