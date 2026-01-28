namespace Shared.Infrastructure.Hubs;


public interface INotification
{

    Task ReceiveShipmentStatusUpdate(ShipmentStatusNotification notification);


    Task ReceiveOrderStatusUpdate(OrderStatusNotification notification);

    Task ReceiveStockAlert(StockAlertNotification notification);

    Task ReceivePaymentUpdate(PaymentStatusNotification notification);


    Task ReceiveNewProductNotification(NewProductNotification notification);


    Task ReceiveNotification(string title, string message, string type);
}

public record ShipmentStatusNotification
{
    public Guid ShipmentId { get; init; }
    public Guid OrderId { get; init; }
    public string Status { get; init; }
    public string TrackingCode { get; init; }
    public string Message { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record OrderStatusNotification
{
    public Guid OrderId { get; init; }
    public string Status { get; init; }
    public string Message { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record StockAlertNotification
{
    public Guid ProductId { get; init; }
    public string Color { get; init; }
    public string Size { get; init; }
    public string Slug { get; init; }
    public string Brand { get; init; }
    public string Message { get; init; }
    public int Stock { get; init; }
    public bool IsAvailable { get; init; }
}

public record PaymentStatusNotification
{
    public Guid OrderId { get; init; }
    public Guid PaymentId { get; init; }
    public string Status { get; init; }
    public string Message { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record NewProductNotification
{
    public Guid ProductId { get; init; }
    public Guid VariantId { get; init; }
    public string Slug { get; init; }
    public string Brand { get; init; }
    public string Sku { get; init; }
    public string ImageUrl { get; init; }
    public string Message { get; init; }
}
