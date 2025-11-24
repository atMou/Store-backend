namespace Inventory.Domain.Models;

public record ProductOutOfStockAlertEvent(ProductId ProductId) : IDomainEvent;