namespace Shared.Domain.Contracts.User;
public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public string? Phone { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public int? Age { get; init; }
    public string? Avatar { get; init; }
    public string? Gender { get; init; }
    public bool IsVerified { get; init; }
    public Guid? CartId { get; init; }
    public AddressDto Address { get; init; }
    public IEnumerable<RoleDto> Roles { get; init; }
    public IEnumerable<string> Permissions { get; init; }
    public IEnumerable<Guid> LikedProductIds { get; init; }
}

public record AddressDto
{
    public string Street { get; init; }
    public string City { get; init; }
    public uint PostalCode { get; init; }
    public uint HouseNumber { get; set; }
    public string? ExtraDetails { get; init; }
}
public record RoleDto
{
    public string Name { get; init; }
    public IEnumerable<string> Permissions { get; init; }
}
