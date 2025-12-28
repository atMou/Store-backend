namespace Identity.Presentation.Requests;

public record VerificationRequest(string Email, bool? RememberMe, string? Token, string? Code);