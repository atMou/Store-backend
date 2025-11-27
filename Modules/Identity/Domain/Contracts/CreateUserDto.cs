namespace Identity.Domain.Contracts;
public record CreateUserDto
{
    public string Email { get; init; } = null!;
    public string? Phone { get; init; }
    public IFormFile? Image { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public byte? Age { get; init; }
    public string Password { get; init; } = null!;
    public string? Gender { get; init; } = null;
    public IEnumerable<string>? Roles { get; init; }
    public string City { get; init; } = null!;
    public string Street { get; init; } = null!;
    public uint PostalCode { get; init; }
    public uint HouseNumber { get; init; }
    public string? ExtraDetails { get; init; }

}
