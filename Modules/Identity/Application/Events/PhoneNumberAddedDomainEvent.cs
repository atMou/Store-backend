namespace Identity.Application.Events;

public record PhoneNumberAddedDomainEvent(Phone Phone, string ConfirmationToken) : IDomainEvent
{

}