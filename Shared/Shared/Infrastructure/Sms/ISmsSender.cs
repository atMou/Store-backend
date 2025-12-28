namespace Shared.Infrastructure.Sms;
public interface ISmsSender
{
	Task SendAsync(string phoneNumber, string message);
}