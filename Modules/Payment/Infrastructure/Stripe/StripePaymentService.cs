using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Payment.Infrastructure.Errors;
using Payment.Infrastructure.Stripe.OptionsSetup;

using Stripe;

namespace Payment.Infrastructure.Stripe;

public class StripePaymentService : IStripePaymentService
{
    private readonly ILogger<StripePaymentService> _logger;
    private readonly StripeSettings _stripeSettings;

    public StripePaymentService(
        IOptions<StripeSettings> stripeSettings,
        ILogger<StripePaymentService> logger)
    {
        _logger = logger;
        _stripeSettings = stripeSettings.Value;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    public async Task<Fin<StripePaymentIntentResponse>> CreatePaymentIntentAsync(
        decimal amount,
        string currency,
        Guid orderId,
        Guid userId,
        string customerEmail,
        CancellationToken cancellationToken = default)
    {

        _logger.LogInformation($"stripe setting {_stripeSettings}", _stripeSettings);

        try
        {
            var service = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe uses smallest currency unit (cents)
                Currency = currency.ToLowerInvariant(),
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
                Metadata = new Dictionary<string, string>
                {
                    { "order_id", orderId.ToString() },
                    { "user_id", userId.ToString() }
                },
                ReceiptEmail = customerEmail,
                Description = $"Payment for Order {orderId}"
            };

            var paymentIntent = await service.CreateAsync(options, cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Created Stripe PaymentIntent {PaymentIntentId} for Order {OrderId}",
                paymentIntent.Id, orderId);

            return new StripePaymentIntentResponse
            {
                PaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                Status = paymentIntent.Status,
                Amount = amount,
                Currency = currency
            };
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex,
                "Stripe error creating payment intent for Order {OrderId}: {ErrorMessage}",
                orderId, ex.Message);
            return Fin<StripePaymentIntentResponse>.Fail(
                BadGatewayError.New($"Stripe error: {ex.StripeError?.Message ?? ex.Message}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error creating payment intent for Order {OrderId}",
                orderId);
            return Fin<StripePaymentIntentResponse>.Fail(
                InternalServerError.New(ex.Message));
        }
    }

    public async Task<Fin<StripePaymentIntentResponse>> GetPaymentIntentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(
                paymentIntentId,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Retrieved Stripe PaymentIntent {PaymentIntentId} with status {Status}",
                paymentIntent.Id, paymentIntent.Status);

            return new StripePaymentIntentResponse
            {
                PaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                Status = paymentIntent.Status,
                Amount = paymentIntent.Amount / 100m,
                Currency = paymentIntent.Currency
            };
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex,
                "Stripe error retrieving payment intent {PaymentIntentId}: {ErrorMessage}",
                paymentIntentId, ex.Message);
            return Fin<StripePaymentIntentResponse>.Fail(
                BadGatewayError.New($"Stripe error: {ex.StripeError?.Message ?? ex.Message}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error retrieving payment intent {PaymentIntentId}",
                paymentIntentId);
            return Fin<StripePaymentIntentResponse>.Fail(
                InternalServerError.New(ex.Message));
        }
    }

    public async Task<Fin<StripeRefundResponse>> RefundPaymentAsync(
        string paymentIntentId,
        decimal? amount = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new RefundService();
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            if (amount.HasValue)
            {
                options.Amount = (long)(amount.Value * 100);
            }

            var refund = await service.CreateAsync(options, cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Created Stripe Refund {RefundId} for PaymentIntent {PaymentIntentId}",
                refund.Id, paymentIntentId);

            return new StripeRefundResponse
            {
                RefundId = refund.Id,
                Status = refund.Status,
                Amount = refund.Amount / 100m,
                Currency = refund.Currency
            };
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex,
                "Stripe error creating refund for PaymentIntent {PaymentIntentId}: {ErrorMessage}",
                paymentIntentId, ex.Message);
            return Fin<StripeRefundResponse>.Fail(
                BadGatewayError.New($"Stripe error: {ex.StripeError?.Message ?? ex.Message}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error creating refund for PaymentIntent {PaymentIntentId}",
                paymentIntentId);
            return Fin<StripeRefundResponse>.Fail(
                InternalServerError.New(ex.Message));
        }
    }

    public async Task<Fin<bool>> CancelPaymentIntentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.CancelAsync(
                paymentIntentId,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Cancelled Stripe PaymentIntent {PaymentIntentId}",
                paymentIntent.Id);

            return true;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex,
                "Stripe error cancelling payment intent {PaymentIntentId}: {ErrorMessage}",
                paymentIntentId, ex.Message);
            return Fin<bool>.Fail(
                BadGatewayError.New($"Stripe error: {ex.StripeError?.Message ?? ex.Message}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error cancelling payment intent {PaymentIntentId}",
                paymentIntentId);
            return Fin<bool>.Fail(
                InternalServerError.New(ex.Message));
        }
    }
}
