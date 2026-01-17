namespace Shared.Infrastructure.Hubs;

/// <summary>
/// Client-side methods that can be invoked by the server
/// </summary>
public interface INotification
{
    /// <summary>
    /// Notify client about shipment status change
    /// </summary>
    Task ReceiveShipmentStatusUpdate(ShipmentStatusNotification notification);

    /// <summary>
    /// Notify client about order status change
    /// </summary>
    Task ReceiveOrderStatusUpdate(OrderStatusNotification notification);

    /// <summary>
    /// Notify client about stock alert
    /// </summary>
    Task ReceiveStockAlert(StockAlertNotification notification);

    /// <summary>
    /// Notify client about payment status
    /// </summary>
    Task ReceivePaymentUpdate(PaymentStatusNotification notification);

    /// <summary>
    /// Notify client about new product arrival
    /// </summary>
    Task ReceiveNewProductNotification(NewProductNotification notification);

    /// <summary>
    /// Generic notification
    /// </summary>
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
