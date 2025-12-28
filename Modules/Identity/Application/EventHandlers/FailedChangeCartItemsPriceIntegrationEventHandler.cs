using Shared.Application.Features.Cart.Events;

namespace Identity.Application.EventHandlers;
internal class FailedChangeCartItemsPriceIntegrationEventHandler : IConsumer<CartItemsPriceChangedIntegrationEvent>
{
	public Task Consume(ConsumeContext<CartItemsPriceChangedIntegrationEvent> context)
	{

		// get admin Users and send them notification about failed change cart items price
		throw new NotImplementedException();
	}
}
