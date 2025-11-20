namespace Identity.Application.EventHandlers;
//internal class UserCreatedEventHandler(IPublishEndpoint endpoint) : INotificationHandler<UserCreatedEvent>
//{
//    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
//    {
//        await endpoint.Publish(new UserCreatedIntegrationEvent(notification.Email, notification.VerificationToken, notification.ExpiresAt), cancellationToken);
//    }
//}