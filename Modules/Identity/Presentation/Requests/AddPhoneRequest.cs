namespace Identity.Presentation.Requests;

public record AddPhoneRequest
{
	public Guid UserId { get; init; }
	public string PhoneNumber { get; init; }
}