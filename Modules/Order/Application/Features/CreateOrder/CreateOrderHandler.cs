

using MassTransit;

using Order.Domain.Contracts;

using Shared.Application.Contracts.Order.Dtos;
using Shared.Application.Features.Order.Events;

namespace Order.Application.Features.CreateOrder;

public class CreateOrderCommand() : ICommand<Fin<Unit>>
{
	public CreateOrderDto CreateOrderDto { get; init; }
}

internal class CreateOrderCommandHandler(OrderDBContext dbContext, IPublishEndpoint endpoint)
	: ICommandHandler<CreateOrderCommand, Fin<Unit>>
{
	public Task<Fin<Unit>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
	{

		var order = Domain.Models.Order.Create(command.CreateOrderDto);

		var db = AddEntity<OrderDBContext, Domain.Models.Order>(order);

		return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken))
			.RaiseOnSuccess(async o =>
			{
				await endpoint.Publish(new OrderCreatedIntegrationEvent
				{
					UserId = o.UserId.Value,
					OrderId = o.Id.Value,
					CartId = o.CartId.Value,
					Total = o.Total,
					Subtotal = o.Subtotal,
					Discount = o.Discount,
					TotalAfterDiscounted = o.TotalAfterDiscounted,
					Tax = o.Tax,
					Address = o.ShippingAddress,
					CouponIds = o.CouponIds.Select(c => c.Value),
					ShipmentCost = o.ShipmentCost,
					OrderItemsDtos = o.OrderItems.Select(oi => new OrderItemDto
					{
						ProductId = oi.ProductId.Value,
						Quantity = oi.Quantity,
						UnitPrice = oi.UnitPrice,
						Slug = oi.Slug,
						ImageUrl = oi.ImageUrl,
						LineTotal = oi.LineTotal,
						Sku = oi.Sku,


					}).ToList(),
				}, cancellationToken);
				return unit;
			});
	}



}