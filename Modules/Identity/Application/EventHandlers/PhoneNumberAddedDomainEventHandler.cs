using Identity.Domain.Events;

using Shared.Infrastructure.Sms;

namespace Identity.Application.EventHandlers;
internal class PhoneNumberAddedDomainEventHandler(ISmsSender smsSender) : INotificationHandler<PhoneNumberAddedDomainEvent>
{
	public async Task Handle(PhoneNumberAddedDomainEvent notification, CancellationToken cancellationToken)
	{
		var message = $"Your confirmation token is: {notification.ConfirmationToken}";
		await smsSender.SendAsync(notification.Phone.Value, message);
	}
}
