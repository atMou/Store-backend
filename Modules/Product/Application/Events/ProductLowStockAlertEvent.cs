namespace Product.Application.Events;
internal record ProductLowStockAlertEvent(ProductId ProductId, int CurrentStock, int LowStockThreshold) : IDomainEvent
{
}
