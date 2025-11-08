namespace Shared.Domain.Contracts.User;
public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public int Age { get; init; }
    public string Avatar { get; init; } = null!;
    public string Gender { get; init; } = null!;
    public bool IsVerified { get; init; }
    public Guid? CartId { get; init; }
    public IEnumerable<Guid> CouponIds { get; init; } = null!;
    public AddressDto Address { get; init; } = null!;
    //public IEnumerable<string> Roles { get; init; } = [];
    public IEnumerable<Guid> LikedProductIds { get; init; } = [];
    public IEnumerable<Guid> OrderIds { get; init; } = [];
}

public record AddressDto
{
    public string Street { get; init; } = null!;
    public string City { get; init; } = null!;
    public string State { get; init; } = null!;
    public string PostalCode { get; init; } = null!;
    public string Country { get; init; } = null!;
}
public record RoleDto
{
    public string Name { get; init; } = null!;
    public IEnumerable<string> Permissions { get; init; } = [];
}
