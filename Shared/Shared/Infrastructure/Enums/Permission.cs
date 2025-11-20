namespace Shared.Infrastructure.Enums;
public enum Permission
{
    BrowseProducts = 1,
    AddToCart = 2,
    PlaceOrder = 3,
    CancelOrder = 4,
    WriteReview = 5,
    ViewOrderHistory = 6,

    CreateProduct = 7,
    EditProduct = 8,
    DeleteProduct = 9,
    ManageOrders = 10,
    ViewPayouts = 11,

    ManageUsers = 12,
    ManageAllProducts = 13,
    ManageAllOrders = 14,
    AccessAnalytics = 15,
    ConfigurePlatform = 16,

    ViewSupportTickets = 17,
    RespondToTickets = 18,
    ResolveDisputes = 19,

    CreateCoupon = 20
}
