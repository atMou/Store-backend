namespace Product.Domain.Events;
internal record ProductLowStockAlertEvent(ProductId ProductId, int CurrentStock, int LowStockThreshold) : IDomainEvent
{
}
