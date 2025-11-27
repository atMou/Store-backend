using Shared.Application.Features.Cart.Events;

namespace Identity.Application.EventHandlers;
internal class FailedChangeCartItemsPriceIntegrationEventHandler : IConsumer<ChangeCartItemsPriceIntegrationEvent>
{
    public Task Consume(ConsumeContext<ChangeCartItemsPriceIntegrationEvent> context)
    {

        // get admin Users and send them notification about failed change cart items price
        throw new NotImplementedException();
    }
}
