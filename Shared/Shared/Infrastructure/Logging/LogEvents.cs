using Microsoft.Extensions.Logging;

namespace Shared.Infrastructure.Logging;

public static class LogEvents
{
    // General (1000-1099)
    public static readonly EventId ApplicationStarting = new(1000, "ApplicationStarting");
    public static readonly EventId ApplicationShutdown = new(1001, "ApplicationShutdown");
    public static readonly EventId ConfigurationError = new(1002, "ConfigurationError");

    // Identity Module (2000-2099)
    public static readonly EventId UserRegistration = new(2000, "UserRegistration");
    public static readonly EventId UserLogin = new(2001, "UserLogin");
    public static readonly EventId EmailVerification = new(2002, "EmailVerification");
    public static readonly EventId PasswordReset = new(2003, "PasswordReset");
    public static readonly EventId UserCreated = new(2004, "UserCreated");
    public static readonly EventId PhoneVerification = new(2005, "PhoneVerification");

    // Product Module (3000-3099)
    public static readonly EventId ProductCreated = new(3000, "ProductCreated");
    public static readonly EventId ProductUpdated = new(3001, "ProductUpdated");
    public static readonly EventId ProductDeleted = new(3002, "ProductDeleted");
    public static readonly EventId PriceChanged = new(3003, "PriceChanged");
    public static readonly EventId StockUpdated = new(3004, "StockUpdated");
    public static readonly EventId ImageUploaded = new(3005, "ImageUploaded");

    // Basket Module (4000-4099)
    public static readonly EventId CartCreated = new(4000, "CartCreated");
    public static readonly EventId CartItemAdded = new(4001, "CartItemAdded");
    public static readonly EventId CartItemRemoved = new(4002, "CartItemRemoved");
    public static readonly EventId CartCheckedOut = new(4003, "CartCheckedOut");
    public static readonly EventId CouponApplied = new(4004, "CouponApplied");
    public static readonly EventId CartPriceUpdated = new(4005, "CartPriceUpdated");
    public static readonly EventId CartCheckoutFailed = new(4006, "CartCheckoutFailed");

    // Order Module (5000-5099)
    public static readonly EventId OrderCreated = new(5000, "OrderCreated");
    public static readonly EventId OrderCancelled = new(5001, "OrderCancelled");
    public static readonly EventId OrderShipped = new(5002, "OrderShipped");
    public static readonly EventId OrderDelivered = new(5003, "OrderDelivered");
    public static readonly EventId OrderCreateFail = new(5003, "OrderCreateFail");


    // Payment Module (6000-6099)
    public static readonly EventId PaymentInitiated = new(6000, "PaymentInitiated");
    public static readonly EventId PaymentCreated = new(6001, "PaymentCreated");
    public static readonly EventId PaymentCompleted = new(6002, "PaymentCompleted");
    public static readonly EventId PaymentFailed = new(6003, "PaymentFailed");
    public static readonly EventId RefundIssued = new(6004, "RefundIssued");
    public static readonly EventId PaymentProcessed = new(6005, "PaymentProcessed");
    public static readonly EventId PaymentCancelled = new(6006, "PaymentCancelled");
    public static readonly EventId PaymentInitiationFailed = new(6007, "PaymentInitiationFailed");

    // Shipment Module (7000-7099)
    public static readonly EventId ShipmentCreated = new(7000, "ShipmentCreated");
    public static readonly EventId ShipmentInTransit = new(7001, "ShipmentInTransit");
    public static readonly EventId ShipmentDelivered = new(7002, "ShipmentDelivered");

    // Inventory Module (8000-8099)
    public static readonly EventId InventoryCreated = new(8000, "InventoryCreated");
    public static readonly EventId StockAdded = new(8001, "StockAdded");
    public static readonly EventId StockReserved = new(8002, "StockReserved");
    public static readonly EventId StockReleased = new(8003, "StockReleased");

    // Integration Events (9000-9099)
    public static readonly EventId IntegrationEventPublished = new(9000, "IntegrationEventPublished");
    public static readonly EventId IntegrationEventReceived = new(9001, "IntegrationEventReceived");
    public static readonly EventId IntegrationEventFailed = new(9002, "IntegrationEventFailed");

    // Database Operations (9100-9199)
    public static readonly EventId DatabaseQueryExecuting = new(9100, "DatabaseQueryExecuting");
    public static readonly EventId DatabaseQueryCompleted = new(9101, "DatabaseQueryCompleted");
    public static readonly EventId DatabaseError = new(9102, "DatabaseError");
    public static readonly EventId DatabaseSaveChanges = new(9103, "DatabaseSaveChanges");
}
