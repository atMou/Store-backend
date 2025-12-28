namespace Shared.Infrastructure.Email;

public interface IEmailService
{
	public K<M, Response> Send<M>(
		EmailAddress from,
		EmailAddress to,
		string subject,
		string plainTextContent,
		string htmlContent,
		CancellationToken token)
		where M : Fallible<M>, MonadIO<M>;
}
