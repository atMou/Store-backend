namespace Identity.Presentation.Requests;

public record VerificationRequest(string email, string token);