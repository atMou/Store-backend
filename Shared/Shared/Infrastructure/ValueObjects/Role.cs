namespace Shared.Infrastructure.ValueObjects;

using static LanguageExt.Prelude;

public record Role
{
    private Role() { }
    public byte Code { get; }
    public string Name { get; }
    public string Description { get; }
    private readonly IEnumerable<Permission> _permissions;
    public IReadOnlyList<Permission> Permissions => _permissions.ToList().AsReadOnly();

    private static readonly List<Role> _all = new();
    public static IReadOnlyList<Role> All => _all;

    private Role(byte code, string name, string description, IEnumerable<Permission> permissions)
    {
        Code = code;
        Name = name;
        Description = description;
        _permissions = permissions;
        _all.Add(this);
    }

    public static Role Default => new(0, nameof(Default), "Unknown User",
        [Permission.BrowseProducts]
        );

    public static Role Customer => new(
        1,
        nameof(Customer),
        "Users who browse, purchase, and review products",
        [
            Permission.BrowseProducts,
                Permission.AddToCart,
                Permission.PlaceOrder,
                Permission.CancelOrder,
                Permission.WriteReview,
                Permission.ViewOrderHistory
        ]);

    public static Role Seller => new(
        2,
        nameof(Seller),
        "Users who list products and manage their own sales",
        [
            Permission.BrowseProducts,
                Permission.CreateProduct,
                Permission.EditProduct,
                Permission.DeleteProduct,
                Permission.ManageOrders,
                Permission.ViewPayouts
        ]);

    public static Role Admin => new(
        3,
        nameof(Admin),
        "Platform administrators with full access",
        Permission.All);

    public static Role Support => new(
        4,
        nameof(Support),
        "Customer service reps handling customer and order inquiries",
        [
            Permission.ViewSupportTickets,
                Permission.RespondToTickets,
                Permission.ResolveDisputes
        ]);

    public static Role Moderator => new(
        5,
        nameof(Moderator),
        "Users who review product listings or resolve disputes",
        [
            Permission.ManageAllProducts,
                Permission.ManageAllOrders,
                Permission.ResolveDisputes
        ]);

    public static Fin<Role> FromCode(byte code) =>
        Optional(_all.FirstOrDefault(role => role.Code == code))
            .ToFin(Error.New($"Invalid role code, '{code}'"));

    public static Fin<Role> FromValue(string name) =>
        Optional(_all.FirstOrDefault(role => role.Name == name))
            .ToFin(Error.New($"Invalid role name, '{name}'"));

    public static Role FromUnsafe(string name) =>
        Optional(_all.FirstOrDefault(role => role.Name == name)).IfNone(() => Role.Default);
    public bool HasPermission(Permission permission) => _permissions.Any(p => p.Code == permission.Code);
}
