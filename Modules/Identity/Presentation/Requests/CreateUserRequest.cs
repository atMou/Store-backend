namespace Identity.Presentation.Requests;

public record RegisterUserRequest
{
    public string Email { get; set; }
    public string? Phone { get; set; }
    public IFormFile? Avatar { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public byte? Age { get; set; }
    public string Password { get; set; }
    public string? Gender { get; set; }
    public bool RememberMe { get; set; }

    public string City { get; set; }
    public string Street { get; set; }
    public uint PostalCode { get; set; }
    public uint HouseNumber { get; set; }
    public string? ExtraDetails { get; set; }

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
            PostalCode = PostalCode,
            HouseNumber = HouseNumber,
            ExtraDetails = ExtraDetails,
            Avatar = Avatar,
            Phone = Phone,
            RememberMe = RememberMe,
        });

    }
}
