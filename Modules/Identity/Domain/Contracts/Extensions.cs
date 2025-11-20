
namespace Identity.Domain.Contracts;

public static class Extensions
{
    public static UserDto ToDto(this User user) => new()
    {
        Id = user.Id.Value,
        Email = user.Email.Value,
        FirstName = user.FirstName.Value,
        LastName = user.LastName.Value,
        Age = user.Age?.Value,
        Avatar = user.Avatar?.Value,
        Gender = user.Gender?.ToString(),
        IsVerified = user.IsEmailVerified,
        CartId = user.CartId?.Value,
        //CouponIds = user.CouponIds.Select(ci => ci.Value),
        Address = new AddressDto
        {
            Street = user.Address.Street,
            City = user.Address.City,
            //State = user.Address.State,
            //PostalCode = user.Address.PostalCode,
            //Country = user.Address.Country
        },
        Roles = user.Roles.Select(r => r.Name),
        //LikedProductIds = user.LikedProducts,
        //OrderIds = user.OrderIds.Select(o => o.Value)
    };
}

