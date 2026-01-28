namespace Inventory.Domain.Events;

public class StockAlertDomainEvent : IDomainEvent
{
    public Guid ProductId { get; init; }
    public string Color { get; init; }
    public string Size { get; init; }
    public string Slug { get; init; }
    public string Message { get; init; }
    public int Stock { get; init; }
    public bool IsAvailable { get; init; }
}