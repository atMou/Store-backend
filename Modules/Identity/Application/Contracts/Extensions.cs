using Address = Identity.Domain.Models.Address;

namespace Identity.Application.Contracts;

public static class Extensions
{
    public static UserResult ToResult(this User user) => new()
    {
        Id = user.Id.Value,
        Email = user.Email.Value,
        Phone = user.Phone?.Value,
        FirstName = user.FirstName.Value,
        LastName = user.LastName.Value,
        Age = user.Age?.Value,
        Avatar = user.Avatar?.Value,
        Gender = user.Gender?.ToString(),
        IsVerified = user.IsEmailVerified,
        CartId = user.CartId?.Value,
        Addresses = user.Addresses.Select(a => a.ToResult()),
        Roles = user.Roles.Select(r => r.ToResult()),
        ProductSubscriptions = user.ProductSubscriptions.Select(subscription => subscription.Key),
        LikedProductIds = user.LikedProducts.Select(likedProduct => likedProduct.ProductId.Value),
        Permissions = user.Permissions.Select(permission => permission.Name)

    };

    private static AddressResult ToResult(this Address address) => new()
    {
        Id = address.Id.Value,
        ReceiverName = address.ReceiverName,
        Street = address.Street,
        City = address.City,
        PostalCode = address.PostalCode,
        HouseNumber = address.HouseNumber,
        ExtraDetails = address.ExtraDetails,
        IsMain = address.IsMain


    };

    private static RoleResult ToResult(this Role role) => new()
    {
        Name = role.Name,
        Permissions = role.Permissions.Select(p => p.Name)
    };

}

