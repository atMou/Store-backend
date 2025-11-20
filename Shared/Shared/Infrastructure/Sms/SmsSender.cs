using Shared.Infrastructure.Sms.Options;

using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Shared.Infrastructure.Sms;

public class SmsSender(IOptions<SmsSettingsOptions> options) : ISmsSender
{
    private readonly SmsSettingsOptions _options = options.Value;

    public async Task SendAsync(string phoneNumber, string message)
    {
        try
        {
            TwilioClient.Init(
                _options.AccountSid,
                _options.AuthToken
            );
            MessageResource mr = await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(_options.From),
                to: new Twilio.Types.PhoneNumber(phoneNumber)
            );


        }
        catch (Exception e)
        {
            Console.WriteLine("''''''''''''''''''''''''''''''###################################################################");
            Console.WriteLine(e);
            throw;
        }
    }
}
