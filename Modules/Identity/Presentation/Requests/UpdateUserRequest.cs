namespace Identity.Presentation.Requests;

public record UpdateUserRequest
{
	public IFormFile? Image { get; init; }
	public string? FirstName { get; init; }
	public string? LastName { get; init; }
	public UpdateAddressDto? Address { get; init; }
	public string? Phone { get; init; }
	public string? Email { get; init; }
	public string? Password { get; init; }
	public string? Gender { get; init; }
	public byte? Age { get; init; }

	public UpdateUserDetailsCommand ToCommand(Guid id)
	{
		return new UpdateUserDetailsCommand()
		{
			UserId = UserId.From(id),
			Email = Email,
			FirstName = FirstName,
			LastName = LastName,
			Address = Address,
			Phone = Phone,
			Age = Age,
			Gender = Gender,
			Password = Password,
			Image = Image
		};
	}

}