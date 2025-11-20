namespace Shared.Infrastructure.Sms.Options;
public class SmsSettingsOptionsSetup(IConfiguration configuration) : IConfigureOptions<SmsSettingsOptions>
{
    private const string _sectionName = "SmsSettings";

    public void Configure(SmsSettingsOptions options) =>
        configuration.GetSection(_sectionName).Bind(options);
}
