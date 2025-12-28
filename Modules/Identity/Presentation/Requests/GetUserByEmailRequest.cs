using Identity.Application.Features.GetUser;

namespace Identity.Presentation.Requests;

public record GetUserByEmailRequest
{
	public string Email { get; set; }

	public GetUserByEmailQuery ToQuery()
	{
		return new GetUserByEmailQuery()
		{
			Email = Email
		};
	}
}