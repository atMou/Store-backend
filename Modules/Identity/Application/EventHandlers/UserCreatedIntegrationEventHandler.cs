using Identity.Application.Events;

namespace Identity.Application.EventHandlers;

public class UserCreatedIntegrationEventHandler(
    IEmailService emailService,
    IEmailTemplateBuilder templateBuilder,
    LinkGenerator link,
    ILogger<UserCreatedIntegrationEventHandler> logger) : IConsumer<UserCreatedIntegrationEvent>
{
    private static readonly Lazy<FileIO> _fileIO = new(() => fileIO.Default);

    public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
    {
        var message = context.Message;

        var verificationLink = BuildVerificationLink(
            message.Email,
            message.VerificationToken!,
            "/"
        );

        var io = from emailBody in templateBuilder.BuildEmailVerificationAsync(
                    message.Name,
                    message.Email,
                    message.VerificationCode!,
                    verificationLink,
                    _fileIO.Value)
                 from response in emailService.Send<IO>(
                    new EmailAddress("ahmedmou@b-tu.de", "Admin"),
                    new EmailAddress(message.Email),
                    "Verify your email",
                    null!,
                    emailBody,
                    context.CancellationToken)
                 select response;

        var res = await io.RunSafeAsync(EnvIO.New(null, context.CancellationToken));

        res.Match(
            Succ: _ => logger.LogInformation(
                "Email verification sent successfully to {Email}",
                message.Email),
            Fail: err => logger.LogError(
                "Error occurred while sending register email verification to {Email}: {Error}",
                message.Email,
                err.Message)
        );
    }

    private static string BuildVerificationLink(string email, string token, string returnUrl)
    {
        return $"http://localhost:3000/users/verify?email={email}&token={token}&returnUrl={Uri.EscapeDataString(returnUrl)}";
    }
}