namespace Identity.Presentation.Requests;

public record VerificationRequest(string Email, string Token);