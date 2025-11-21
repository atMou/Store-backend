
namespace Identity.Domain.Contracts;

public static class Extensions
{
    public static UserDto ToDto(this User user) => new()
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
        Address = user.Address.ToDto(),
        Roles = user.Roles.Select(r => r.ToDto()),
        Permissions = user.Permissions.Select(p => p.Name),
        LikedProductIds = user.LikedProducts.Select(id => id.ProductId.Value),


    };
    public static AddressDto ToDto(this Address address) => new()
    {
        Street = address.Street,
        City = address.City,
        PostalCode = address.PostalCode,
        HouseNumber = address.HouseNumber,
        ExtraDetails = address.ExtraDetails

    };

    public static RoleDto ToDto(this Role role) => new()
    {
        Name = role.Name,
        Permissions = role.Permissions.Select(p => p.Name)
    };

}

