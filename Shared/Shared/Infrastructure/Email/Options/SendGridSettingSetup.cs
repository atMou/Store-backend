

namespace Shared.Infrastructure.Email.Options;

public class SendGridSettingSetup(IConfiguration configuration) : IConfigureOptions<SendGridSettings>
{
    private const string _sectionName = "SendGrid";

    public void Configure(SendGridSettings options) =>
        configuration.GetSection(_sectionName).Bind(options);

}
