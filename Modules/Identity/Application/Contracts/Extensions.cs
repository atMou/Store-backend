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
        Permissions = user.Permissions.Select(p => p.Name),
        LikedProductIds = user.LikedProducts.Select(id => id.ProductId.Value),


    };
    public static AddressResult ToResult(this Address address) => new()
    {
        Street = address.Street,
        City = address.City,
        PostalCode = address.PostalCode,
        HouseNumber = address.HouseNumber,
        ExtraDetails = address.ExtraDetails,
        IsMain = address.IsMain


    };

    public static RoleResult ToResult(this Role role) => new()
    {
        Name = role.Name,
        Permissions = role.Permissions.Select(p => p.Name)
    };

}

