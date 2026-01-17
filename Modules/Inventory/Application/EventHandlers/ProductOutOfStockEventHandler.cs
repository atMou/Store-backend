namespace Inventory.Application.EventHandlers;

//public class ProductOutOfStockEventHandler(IHubContext<NotificationHub, INotificationClient> hub, ISender sender, IClock clock, InventoryDbContext inventoryDbContext, ILogger<ProductOutOfStockEventHandler> logger) : INotificationHandler<ProductOutOfStockDomainEvent>
//{
//    public async Task Handle(ProductOutOfStockDomainEvent notification, CancellationToken cancellationToken)
//    {
//        var results = await sender.Send(new GetUsersWithOutOfStockCartItemsQuery(notification.Id), cancellationToken);
//        Fin<IEnumerable<StockSubscription>> fin = results.Map(ids => ids.Select(id => StockSubscription.Create(
//            notification.ProductId,
//            notification.Id,
//            notification.StockLevel,
//            notification.Message,
//            clock
//        )));
//        //hub.Clients.Users().OutOfStock()
//        var db = fin.Match(
//            AddEntities<InventoryDbContext, StockSubscription>,
//            Db<InventoryDbContext, Unit>.Fail);

//        await db.RunSaveAsync(inventoryDbContext, EnvIO.New(null, cancellationToken)).RaiseOnFail(err => logger.LogCritical($"An error occurred while saving the database: {err}"));

//    }
//}