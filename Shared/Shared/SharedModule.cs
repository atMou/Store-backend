using Shared.Infrastructure.Clock;
using Shared.Infrastructure.Email;
using Shared.Infrastructure.Images;
using Shared.Infrastructure.Sms;
using Shared.Infrastructure.Sms.Options;

namespace Shared;

public static class SharedModule
{
    public static IServiceCollection AddSharedModule(this IServiceCollection services, IConfiguration configuration)
    {

        services.ConfigureOptions<SmsSettingsOptionsSetup>();
        services.ConfigureOptions<SendGridSettingSetup>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<ISmsSender, SmsSender>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IClock, Clock>();


        return services;
    }

}
