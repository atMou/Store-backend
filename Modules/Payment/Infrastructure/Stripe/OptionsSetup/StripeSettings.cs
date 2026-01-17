namespace Payment.Infrastructure.Stripe.OptionsSetup;

public class StripeSettings
{


    public string PublishableKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public string WebhookSecret { get; set; } = null!;
    public string Currency { get; set; } = "eur";
}
