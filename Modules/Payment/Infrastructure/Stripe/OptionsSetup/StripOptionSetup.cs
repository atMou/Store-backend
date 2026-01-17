using Microsoft.Extensions.Options;

namespace Payment.Infrastructure.Stripe.OptionsSetup;

public record StripOptionSetup(IConfiguration Configuration) : IConfigureOptions<StripeSettings>
{
    private const string _sectionName = "Stripe";
    public void Configure(StripeSettings options)
    {
        Configuration.GetSection(_sectionName).Bind(options);
    }
}
