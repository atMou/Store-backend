namespace Identity.Application.EventHandlers;

public class UserCreatedIntegrationEventHandler(IEmailService emailService, LinkGenerator link) : IConsumer<UserCreatedIntegrationEvent>
{
    private static Lazy<FileIO> _fileIO => new(() => fileIO.Default);

    public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
    {
        var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Templates",
            "Email.html"
        );

        var io = from template in _fileIO.Value.Exists(path)
                .Bind(exists =>
                    exists
                        ? _fileIO.Value.ReadAllText(path, Encoding.UTF8)
                        : IO.fail<string>(Error.New("path  does not exists")))

                 let emailBody = template
                     .Replace("{{VerificationLink}}",
                         BuildVerificationLink(
                             context.Message.Email,
                             context.Message.VerificationToken.ToString()!))
                     .Replace("{{UserEmail}}", context.Message.Email)

                 from response in emailService.Send<IO>(
                     new EmailAddress("ahmedmou@b-tu.de", "Admin"),
                     new EmailAddress(context.Message.Email),
                     "VerifyEmail your email",
                     null!,
                     emailBody, context.CancellationToken)
                 select response;

        var res = await io.RunAsync(EnvIO.New(null, context.CancellationToken));
        var body = await res.Body.ReadAsStringAsync();
        Console.WriteLine($"SENDGRID STATUS: {res.StatusCode}");
        Console.WriteLine($"SENDGRID RESPONSE: {body}");
    }


    public string BuildVerificationLink(string email, string token)
    {
        return $"http://localhost:5046/users/verify?email={email}&RefreshToken={token}";
    }

}
