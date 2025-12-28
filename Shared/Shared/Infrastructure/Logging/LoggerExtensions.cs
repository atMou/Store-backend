using Microsoft.Extensions.Logging;

namespace Shared.Infrastructure.Logging;

/// <summary>
/// Extension methods for structured logging following best practices
/// </summary>
public static class LoggerExtensions
{
    // Identity Module Logging
    public static void LogUserRegistration(this ILogger logger, Guid userId, string email)
    {
        logger.LogInformation(
            LogEvents.UserRegistration,
            "User registration initiated for {Email} with UserId: {UserId}",
            email, userId);
    }

    public static void LogUserLogin(this ILogger logger, Guid userId, string email, bool success)
    {
        if (success)
        {
            logger.LogInformation(
                LogEvents.UserLogin,
                "User {UserId} ({Email}) logged in successfully",
                userId, email);
        }
        else
        {
            logger.LogWarning(
                LogEvents.UserLogin,
                "Failed login attempt for {Email}",
                email);
        }
    }

    public static void LogEmailVerification(this ILogger logger, Guid userId, bool success)
    {
        if (success)
        {
            logger.LogInformation(
                LogEvents.EmailVerification,
                "Email verified successfully for User {UserId}",
                userId);
        }
        else
        {
            logger.LogWarning(
                LogEvents.EmailVerification,
                "Email verification failed for User {UserId}",
                userId);
        }
    }

    // Product Module Logging
    public static void LogProductCreated(this ILogger logger, Guid productId, string slug, decimal price)
    {
        logger.LogInformation(
            LogEvents.ProductCreated,
            "Product created: {ProductId}, Slug: {Slug}, Price: {Price}",
            productId, slug, price);
    }

    public static void LogProductUpdated(this ILogger logger, Guid productId, object changes)
    {
        logger.LogInformation(
            LogEvents.ProductUpdated,
            "Product {ProductId} updated. Changes: {@Changes}",
            productId, changes);
    }

    public static void LogPriceChanged(this ILogger logger, Guid productId, decimal oldPrice, decimal newPrice)
    {
        logger.LogInformation(
            LogEvents.PriceChanged,
            "Price changed for Product {ProductId}: {OldPrice} -> {NewPrice}",
            productId, oldPrice, newPrice);
    }

    public static void LogImageOperation(this ILogger logger, string operation, int imageCount, Guid? productId = null)
    {
        logger.LogInformation(
            LogEvents.ImageUploaded,
            "Image {Operation}: {ImageCount} images for Product {ProductId}",
            operation, imageCount, productId);
    }

    // Basket Module Logging
    public static void LogCartCreated(this ILogger logger, Guid userId, Guid cartId)
    {
        logger.LogInformation(
            LogEvents.CartCreated,
            "Cart {CartId} created for User {UserId}",
            cartId, userId);
    }

    public static void LogCartItemAdded(this ILogger logger, Guid cartId, Guid productId, int quantity)
    {
        logger.LogInformation(
            LogEvents.CartItemAdded,
            "Item added to Cart {CartId}: Product {ProductId}, Quantity: {Quantity}",
            cartId, productId, quantity);
    }

    public static void LogCouponApplied(this ILogger logger, Guid cartId, Guid couponId, decimal discount)
    {
        logger.LogInformation(
            LogEvents.CouponApplied,
            "Coupon {CouponId} applied to Cart {CartId}, Discount: {Discount}",
            couponId, cartId, discount);
    }

    // Order Module Logging
    public static void LogOrderCreated(this ILogger logger, Guid orderId, Guid userId, decimal total)
    {
        logger.LogInformation(
            LogEvents.OrderCreated,
            "Order {OrderId} created for User {UserId}, Total: {Total}",
            orderId, userId, total);
    }

    public static void LogOrderStatusChanged(this ILogger logger, Guid orderId, string oldStatus, string newStatus)
    {
        logger.LogInformation(
            "Order {OrderId} status changed: {OldStatus} -> {NewStatus}",
            orderId, oldStatus, newStatus);
    }

    // Integration Events Logging
    public static void LogIntegrationEventPublished(this ILogger logger, string eventType, object eventData)
    {
        logger.LogInformation(
            LogEvents.IntegrationEventPublished,
            "Integration event published: {EventType}, Data: {@EventData}",
            eventType, eventData);
    }

    public static void LogIntegrationEventReceived(this ILogger logger, string eventType, Guid? correlationId = null)
    {
        logger.LogInformation(
            LogEvents.IntegrationEventReceived,
            "Integration event received: {EventType}, CorrelationId: {CorrelationId}",
            eventType, correlationId);
    }

    public static void LogIntegrationEventFailed(this ILogger logger, string eventType, Exception ex)
    {
        logger.LogError(
            LogEvents.IntegrationEventFailed,
            ex,
            "Integration event processing failed: {EventType}",
            eventType);
    }

    // Database Operations Logging
    public static void LogDatabaseOperation(this ILogger logger, string operation, string entityType, int affectedRows)
    {
        logger.LogDebug(
            LogEvents.DatabaseSaveChanges,
            "Database {Operation} completed: {EntityType}, Affected rows: {AffectedRows}",
            operation, entityType, affectedRows);
    }

    public static void LogDatabaseError(this ILogger logger, Exception ex, string operation)
    {
        logger.LogError(
            LogEvents.DatabaseError,
            ex,
            "Database operation failed: {Operation}",
            operation);
    }
}
