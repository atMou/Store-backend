namespace Identity.Infrastructure.OptionsSetup;

public class JwtOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtOptions>
{
    private const string _sectionName = "JwtOptions";

    public void Configure(JwtOptions options) =>
        configuration.GetSection(_sectionName).Bind(options);
}
