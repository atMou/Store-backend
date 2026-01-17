using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Shared.Infrastructure.Hubs.Services;

public interface INotificationService
{
    Task NotifyShipmentStatusChanged(Guid userId, ShipmentStatusNotification notification);
    Task NotifyOrderStatusChanged(Guid userId, OrderStatusNotification notification);
    Task NotifyStockAlert(Guid userId, StockAlertNotification notification);
    Task NotifyStockAlert(StockAlertNotification notification);
    Task NotifyPaymentUpdate(Guid userId, PaymentStatusNotification notification);
    Task SendNotification(Guid userId, string title, string message, string type);

    // Broadcast to groups
    Task BroadcastToRole(string role, string title, string message, string type);
    Task BroadcastToOrder(Guid orderId, string title, string message, string type);

    // Broadcast to all users
    Task BroadcastNewProductArrival(NewProductNotification notification);
    Task BroadcastToAllUsers(string title, string message, string type);
}

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub, INotification> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<NotificationHub, INotification> hubContext,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyShipmentStatusChanged(Guid userId, ShipmentStatusNotification notification)
    {
        try
        {
            await _hubContext.Clients
                .Group($"user_{userId}")
                .ReceiveShipmentStatusUpdate(notification);

            // Also notify shipment-specific subscribers
            await _hubContext.Clients
                .Group($"shipment_{notification.ShipmentId}")
                .ReceiveShipmentStatusUpdate(notification);

            _logger.LogInformation($"Shipment status notification sent to user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending shipment notification to user {userId}");
        }
    }

    public async Task NotifyOrderStatusChanged(Guid userId, OrderStatusNotification notification)
    {
        try
        {
            await _hubContext.Clients
                .Group($"user_{userId}")
                .ReceiveOrderStatusUpdate(notification);

            // Also notify order-specific subscribers
            await _hubContext.Clients
                .Group($"order_{notification.OrderId}")
                .ReceiveOrderStatusUpdate(notification);

            _logger.LogInformation($"Order status notification sent to user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending order notification to user {userId}");
        }
    }


    public async Task NotifyStockAlert(Guid userId, StockAlertNotification notification)
    {
        try
        {
            await _hubContext.Clients
                .Group($"user_{userId}")
                .ReceiveStockAlert(notification);

            // Also notify product-specific subscribers
            await _hubContext.Clients
                .Group($"product_{notification.ProductId}")
                .ReceiveStockAlert(notification);

            _logger.LogInformation($"Stock alert sent to user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending stock alert to user {userId}");
        }
    }

    public async Task NotifyStockAlert(StockAlertNotification notification)
    {
        try
        {
            await _hubContext.Clients.Group($"product_{notification.ProductId}_{notification.Size}")
                .ReceiveStockAlert(notification);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error sending stock alert for product {notification.ProductId} with size {notification.Size}");
            throw;
        }
    }

    public async Task NotifyPaymentUpdate(Guid userId, PaymentStatusNotification notification)
    {
        try
        {
            await _hubContext.Clients
                .Group($"user_{userId}")
                .ReceivePaymentUpdate(notification);

            _logger.LogInformation($"Payment notification sent to user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending payment notification to user {userId}");
        }
    }

    public async Task SendNotification(Guid userId, string title, string message, string type)
    {
        try
        {
            await _hubContext.Clients
                .Group($"user_{userId}")
                .ReceiveNotification(title, message, type);

            _logger.LogInformation($"Generic notification sent to user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending notification to user {userId}");
        }
    }

    public async Task BroadcastToRole(string role, string title, string message, string type)
    {
        try
        {
            await _hubContext.Clients
                .Group($"role_{role}")
                .ReceiveNotification(title, message, type);

            _logger.LogInformation($"Notification broadcast to role {role}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error broadcasting to role {role}");
        }
    }

    public async Task BroadcastToOrder(Guid orderId, string title, string message, string type)
    {
        try
        {
            await _hubContext.Clients
                .Group($"order_{orderId}")
                .ReceiveNotification(title, message, type);

            _logger.LogInformation($"Notification broadcast to order {orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error broadcasting to order {orderId}");
        }
    }

    public async Task BroadcastNewProductArrival(NewProductNotification notification)
    {
        try
        {
            // Send to ALL connected clients
            await _hubContext.Clients.All.ReceiveNewProductNotification(notification);

            _logger.LogInformation($"New product notification broadcast to all users: {notification.Slug}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error broadcasting new product notification: {notification.Slug}");
        }
    }

    public async Task BroadcastToAllUsers(string title, string message, string type)
    {
        try
        {
            // Send generic notification to ALL connected clients
            await _hubContext.Clients.All.ReceiveNotification(title, message, type);

            _logger.LogInformation($"Generic notification broadcast to all users: {title}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error broadcasting to all users");
        }
    }

}
