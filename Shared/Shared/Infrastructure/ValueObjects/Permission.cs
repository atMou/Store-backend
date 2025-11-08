namespace Shared.Infrastructure.ValueObjects;

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

    // Customer actions
    public static Permission BrowseProducts =>
        new(1, nameof(BrowseProducts), "View product catalog");

    public static Permission AddToCart =>
        new(2, nameof(AddToCart), "Add items to shopping cart");

    public static Permission PlaceOrder =>
        new(3, nameof(PlaceOrder), "Place an order");

    public static Permission CancelOrder =>
        new(4, nameof(CancelOrder), "Cancel own order");

    public static Permission WriteReview =>
        new(5, nameof(WriteReview), "Write a review for purchased products");

    public static Permission ViewOrderHistory =>
        new(6, nameof(ViewOrderHistory), "Access order history");

    // Seller actions
    public static Permission CreateProduct =>
        new(7, nameof(CreateProduct), "List a new product");

    public static Permission EditProduct =>
        new(8, nameof(EditProduct), "Modify own product listings");

    public static Permission DeleteProduct =>
        new(9, nameof(DeleteProduct), "Remove own product listings");

    public static Permission ManageOrders =>
        new(10, nameof(ManageOrders), "Fulfill and manage orders for own products");

    public static Permission ViewPayouts =>
        new(11, nameof(ViewPayouts), "View sales and payouts");

    // Admin actions
    public static Permission ManageUsers =>
        new(12, nameof(ManageUsers), "Create, edit, or ban users");

    public static Permission ManageAllProducts =>
        new(13, nameof(ManageAllProducts), "Edit or delete any product");

    public static Permission ManageAllOrders =>
        new(14, nameof(ManageAllOrders), "Manage orders across the platform");

    public static Permission AccessAnalytics =>
        new(15, nameof(AccessAnalytics), "View platform analytics and reports");

    public static Permission ConfigurePlatform =>
        new(16, nameof(ConfigurePlatform), "Modify app settings (e.g., fees, policies)");

    // Support / Ops
    public static Permission ViewSupportTickets =>
        new(17, nameof(ViewSupportTickets), "Access customer inquiries");

    public static Permission RespondToTickets =>
        new(18, nameof(RespondToTickets), "Reply to support tickets");

    public static Permission ResolveDisputes =>
        new(19, nameof(ResolveDisputes), "Handle order and product disputes");



    public static Fin<Permission> FromCode(byte code) =>
        Optional(_all.FirstOrDefault(p => p.Code == code))
            .ToFin(Error.New($"Invalid permission code: {code}"));

    public static Fin<Permission> FromValue(string name) =>
        Optional(_all.FirstOrDefault(p => p.Name == name))
            .ToFin(Error.New($"Invalid permission name: {name}"));

    public static Permission FromUnsafe(string name) =>
        Optional(_all.FirstOrDefault(p => p.Name == name)).IfNone(() => BrowseProducts);
}
