namespace Identity.Presentation.Requests;

public record ResendVerificationRequest
{
    public string Email { get; set; }
}