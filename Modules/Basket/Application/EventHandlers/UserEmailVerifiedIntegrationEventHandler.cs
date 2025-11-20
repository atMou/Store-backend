using Shared.Messaging.Events;

namespace Basket.Application.EventHandlers;

public class UserEmailVerifiedIntegrationEventHandler(ISender sender, IPublishEndpoint endpoint) :
    IConsumer<UserEmailVerifiedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserEmailVerifiedIntegrationEvent> context)
    {

        // get the taxrate base on user location or default taxrate
        decimal tax = 0.15M;
        var result = await sender.Send(new CreateCartCommand() { UserId = UserId.From(context.Message.UserId), TaxRate = tax });

        if (result.IsFail)
        {
            string[] errors = result.FailSpan().ToArray().Select(err => err.Message).ToArray();
            await endpoint.Publish(new FailCreateCartIntegrationEvent(context.Message.UserId, errors));
        }
    }
}