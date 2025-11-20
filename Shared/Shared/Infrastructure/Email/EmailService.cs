
namespace Shared.Infrastructure.Email;

public class EmailService(IOptions<SendGridSettings> options) : IEmailService
{
    private readonly SendGridSettings _options = options.Value;

    public K<M, Response> Send<M>(
        EmailAddress from,
        EmailAddress to,
        string subject,
        string plainTextContent,
        string htmlContent,
        CancellationToken token)
        where M : Fallible<M>, MonadIO<M>
    {
        return from key in Optional(_options.ApiKey).Match(M.Pure,
                () => M.Fail<string>(InvalidOperationError.New("SendGrid API Key is not configured.")))
               from response in M.LiftIO(IO.liftAsync(e =>
                   Send(key, @from, to, subject, plainTextContent, htmlContent, token)))
               select response;
    }

    private async Task<Response> Send(
        string apiKey,
        EmailAddress from,
        EmailAddress to,
        string subject,
        string plainTextContent,
        string htmlContent,
        CancellationToken token)
    {
        var client = new SendGridClient(apiKey);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        return await client.SendEmailAsync(msg, token);
    }
}