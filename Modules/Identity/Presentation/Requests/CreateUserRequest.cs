namespace Identity.Presentation.Requests;

public record CreateUserRequest
{
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? Avatar { get; init; } = null;
    public byte? Age { get; init; }
    public string Password { get; init; } = null!;
    public string? Gender { get; init; } = null;
    //public IEnumerable<string>? Roles { get; init; }
    public string City { get; init; } = null!;
    public string Street { get; init; } = null!;
    public uint ZipCode { get; init; }
    public uint HouseNumber { get; init; }
    public string? ExtraDetails { get; init; }

    public RegisterCommand ToCommand()
    {
        return new RegisterCommand(new CreateUserDto()
        {
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            Age = Age,
            Password = Password,
            Gender = Gender,
            City = City,
            Street = Street,
            ZipCode = ZipCode,
            HouseNumber = HouseNumber,
            ExtraDetails = ExtraDetails

        });

    }
}
