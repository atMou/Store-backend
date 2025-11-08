namespace Identity.Domain.Contracts;
public record CreateUserDto
{
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public byte Age { get; init; }
    public string Password { get; init; } = null!;
    public string Gender { get; init; } = null!;
    public IEnumerable<string>? Roles { get; init; }
    public string City { get; init; } = null!;
    public string Street { get; init; } = null!;
    public uint ZipCode { get; init; }
    public short HouseNumber { get; init; }
    public string? ExtraDetails { get; init; }
}
