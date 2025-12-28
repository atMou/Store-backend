namespace Identity.Domain.Contracts;

public class UpdateUserDto
{
	public UserId UserId { get; init; } = null!;
	public ImageUrl? Image { get; init; }
	public string? FirstName { get; init; }
	public string? LastName { get; init; }
	public UpdateAddressDto? AddressDto { get; init; }
	public string? Phone { get; init; }
	public string? Email { get; init; }
	public string? Password { get; init; }
	public string? Gender { get; init; }
	public byte? Age { get; init; }
}
