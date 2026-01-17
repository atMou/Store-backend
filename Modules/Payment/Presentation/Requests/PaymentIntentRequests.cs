using Payment.Application.Features.ConfirmStripePaymentIntent;
using Payment.Application.Features.CancelStripePaymentIntent;

namespace Payment.Presentation.Requests;

public record ConfirmStripePaymentIntentRequest
{
    public Guid PaymentId { get; init; }

    public ConfirmStripePaymentIntentCommand ToCommand() =>
        new ConfirmStripePaymentIntentCommand
        {
            PaymentId = PaymentId
        };
}

public record CancelStripePaymentIntentRequest
{
    public Guid PaymentId { get; init; }

    public CancelStripePaymentIntentCommand ToCommand() =>
        new CancelStripePaymentIntentCommand
        {
            PaymentId = PaymentId
        };
}
