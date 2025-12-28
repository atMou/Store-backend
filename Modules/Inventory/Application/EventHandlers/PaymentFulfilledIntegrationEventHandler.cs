using MediatR;

using Microsoft.Extensions.Logging;

using Shared.Application.Contracts.Order.Queries;
using Shared.Application.Features.Payment.Events;
using Shared.Infrastructure.Clock;

namespace Inventory.Application.EventHandlers;

public class PaymentFulfilledIntegrationEventHandler(
    InventoryDbContext dbContext,
    ILogger<PaymentFulfilledIntegrationEventHandler> logger,
    IClock clock,
    ISender sender
) : IConsumer<PaymentFulfilledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentFulfilledIntegrationEvent> context)
    {
        var db = from result in IO.liftAsync(async e => await sender.Send(new GetOrderByIdQuery
        {
            OrderId = context.Message.OrderId
        }, e.Token))
                 from or in result
                 let t = or.OrderItems.Select(oir => (ProductId.From(oir.ProductId), oir.Quantity))
                 from inventory in
                     GetUpdateEntities<InventoryDbContext, Domain.Models.Inventory>
                     (inventory => t.Select(tuple => tuple.Item1).Contains(inventory.ProductId)
                         ,
                         null,
                         inventory =>
                         {
                             foreach (var tuple in t)
                             {
                                 if (tuple.Item1 == inventory.ProductId)
                                 {
                                     return inventory.ReleaseReservation(tuple.Quantity);
                                 }
                             }

                             return inventory;
                         }
                     )
                 select unit;

        await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken))
            .RaiseOnFail(err =>
                logger.LogError(
                    $"Error releasing reservation due to order completion. Error details: {err}"));
    }
}