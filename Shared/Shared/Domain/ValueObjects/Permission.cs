namespace Shared.Domain.ValueObjects;

public record Permission
{
    private Permission() { }

    public byte Code { get; }
    public string Name { get; }
    public string Description { get; }

    private static readonly List<Permission> _all = new();
    public static IReadOnlyList<Permission> All => _all;

    private Permission(byte code, string name, string description)
    {
        Code = code;
        Name = name;
        Description = description;
        _all.Add(this);
    }

    public static readonly Permission BrowseProducts =
           new(1, nameof(BrowseProducts), "View product catalog");

    public static readonly Permission AddToCart =
        new(2, nameof(AddToCart), "Add items to shopping cart");

    public static readonly Permission PlaceOrder =
        new(3, nameof(PlaceOrder), "Place an order");

    public static readonly Permission CancelOrder =
        new(4, nameof(CancelOrder), "Cancel own order");

    public static readonly Permission WriteReview =
        new(5, nameof(WriteReview), "Write a review for purchased products");

    public static readonly Permission ViewOrderHistory =
        new(6, nameof(ViewOrderHistory), "Access order history");

    public static readonly Permission CreateProduct =
        new(7, nameof(CreateProduct), "List a new product");

    public static readonly Permission EditProduct =
        new(8, nameof(EditProduct), "Modify own product listings");

    public static readonly Permission DeleteProduct =
        new(9, nameof(DeleteProduct), "Remove own product listings");

    public static readonly Permission ManageOrders =
        new(10, nameof(ManageOrders), "Fulfill and manage orders for own products");

    public static readonly Permission ViewPayouts =
        new(11, nameof(ViewPayouts), "View sales and payouts");

    public static readonly Permission ManageUsers =
        new(12, nameof(ManageUsers), "Create, edit, or ban users");

    public static readonly Permission ManageAllProducts =
        new(13, nameof(ManageAllProducts), "Edit or delete any product");

    public static readonly Permission ManageAllOrders =
        new(14, nameof(ManageAllOrders), "Manage orders across the platform");

    public static readonly Permission AccessAnalytics =
        new(15, nameof(AccessAnalytics), "View platform analytics and reports");

    public static readonly Permission ConfigurePlatform =
        new(16, nameof(ConfigurePlatform), "Modify app settings (e.g., fees, policies)");

    public static readonly Permission ViewSupportTickets =
        new(17, nameof(ViewSupportTickets), "Access customer inquiries");

    public static readonly Permission RespondToTickets =
        new(18, nameof(RespondToTickets), "Reply to support tickets");

    public static readonly Permission ResolveDisputes =
        new(19, nameof(ResolveDisputes), "Handle order and product disputes");

    public static readonly Permission ManageCoupons =
        new(20, nameof(ManageCoupons), "Can Manage coupons");



    public static readonly Permission ManageCarts =
        new(21, nameof(ManageCarts), "Can Manage carts");

    public static readonly Permission ManageInventory =
        new(22, nameof(ManageInventory), "Can Manage inventory");

    public static readonly Permission MakeRefund =
        new(23, nameof(MakeRefund), "Can Make Refund");

    public static readonly Permission ViewDashboard =
        new(24, nameof(ViewDashboard), "Can view Dashboard");

    public static Fin<Permission> FromCode(byte code) =>
        Optional(_all.FirstOrDefault(p => p.Code == code))
            .ToFin(Error.New($"Invalid permission code: {code}"));

    public static Fin<Permission> FromValue(string name) =>
        Optional(_all.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.CurrentCultureIgnoreCase)))
            .ToFin(Error.New($"Invalid permission name: {name}"));

    public static Permission FromUnsafe(string name)
    {
        var result =
            Optional(_all.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.CurrentCultureIgnoreCase))).IfNone(() => BrowseProducts);
        return result;
    }
}
