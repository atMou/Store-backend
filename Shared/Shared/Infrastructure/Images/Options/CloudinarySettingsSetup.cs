namespace Shared.Infrastructure.Images.Options;

public class CloudinarySettingsSetup(IConfiguration configuration)
    : IConfigureOptions<CloudinarySettings>
{
    private const string SectionName = "Cloudinary";

    public void Configure(CloudinarySettings options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}
