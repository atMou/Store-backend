using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Payment.Infrastructure.Errors;
using Payment.Infrastructure.Stripe.OptionsSetup;

using Stripe;

namespace Payment.Application.Features.StripeWebhook;

public record HandleStripeWebhookCommand : ICommand<Fin<Unit>>
{
    public string Payload { get; init; } = null!;
    public string Signature { get; init; } = null!;
}

internal class HandleStripeWebhookCommandHandler(
    ISender sender,
    IOptions<StripeSettings> stripeSettings,
    ILogger<HandleStripeWebhookCommandHandler> logger)
    : ICommandHandler<HandleStripeWebhookCommand, Fin<Unit>>
{
    private readonly StripeSettings _stripeSettings = stripeSettings.Value;

    public async Task<Fin<Unit>> Handle(
        HandleStripeWebhookCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                request.Payload,
                request.Signature,
                _stripeSettings.WebhookSecret);

            logger.LogInformation(
                "Processing Stripe webhook event: {EventType} - {EventId}",
                stripeEvent.Type, stripeEvent.Id);

            return stripeEvent.Type switch
            {
                "payment_intent.succeeded" => await HandlePaymentIntentSucceeded(stripeEvent, cancellationToken),
                "payment_intent.payment_failed" => await HandlePaymentIntentFailed(stripeEvent, cancellationToken),
                "payment_intent.canceled" => await HandlePaymentIntentCanceled(stripeEvent, cancellationToken),
                _ => HandleUnknownEvent(stripeEvent)
            };
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe webhook signature verification failed");
            return Fin<Unit>.Fail(BadRequestError.New("Invalid webhook signature"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Stripe webhook");
            return FinFail<Unit>(InternalServerError.New(ex.Message));
        }
    }

    private async Task<Fin<Unit>> HandlePaymentIntentSucceeded(
        Event stripeEvent,
        CancellationToken cancellationToken)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        if (paymentIntent == null)
        {
            logger.LogWarning("PaymentIntent data is null in event {EventId}", stripeEvent.Id);
            return unit;
        }

        logger.LogInformation(
            "Payment succeeded for PaymentIntent {PaymentIntentId}",
            paymentIntent.Id);

        // Extract order ID from metadata
        if (!paymentIntent.Metadata.TryGetValue("order_id", out var orderIdStr) ||
            !Guid.TryParse(orderIdStr, out var orderId))
        {
            logger.LogWarning(
                "Could not extract order_id from PaymentIntent {PaymentIntentId} metadata",
                paymentIntent.Id);
            return unit;
        }

        // Find the payment by StripePaymentIntentId and mark as fulfilled
        // This would require querying the database to find the payment
        // For now, we'll log it and return success
        logger.LogInformation(
            "Would mark payment as fulfilled for Order {OrderId} and PaymentIntent {PaymentIntentId}",
            orderId, paymentIntent.Id);

        return unit;
    }

    private Task<Fin<Unit>> HandlePaymentIntentFailed(
        Event stripeEvent,
        CancellationToken cancellationToken)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        if (paymentIntent == null)
        {
            logger.LogWarning("PaymentIntent data is null in event {EventId}", stripeEvent.Id);
            return Task.FromResult<Fin<Unit>>(unit);
        }

        logger.LogWarning(
            "Payment failed for PaymentIntent {PaymentIntentId}. Reason: {FailureMessage}",
            paymentIntent.Id,
            paymentIntent.LastPaymentError?.Message ?? "Unknown");

        return Task.FromResult<Fin<Unit>>(unit);
    }

    private Task<Fin<Unit>> HandlePaymentIntentCanceled(
        Event stripeEvent,
        CancellationToken cancellationToken)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        if (paymentIntent == null)
        {
            logger.LogWarning("PaymentIntent data is null in event {EventId}", stripeEvent.Id);
            return Task.FromResult<Fin<Unit>>(unit);
        }

        logger.LogInformation(
            "Payment canceled for PaymentIntent {PaymentIntentId}",
            paymentIntent.Id);

        return Task.FromResult<Fin<Unit>>(unit);
    }

    private Fin<Unit> HandleUnknownEvent(Event stripeEvent)
    {
        logger.LogInformation(
            "Unhandled Stripe event type: {EventType}",
            stripeEvent.Type);
        return unit;
    }
}
