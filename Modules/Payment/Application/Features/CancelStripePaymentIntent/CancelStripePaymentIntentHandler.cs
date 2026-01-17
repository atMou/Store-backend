using Payment.Infrastructure.Stripe;
using Shared.Infrastructure.Errors;
using Shared.Infrastructure.Logging;
using Microsoft.Extensions.Logging;

namespace Payment.Application.Features.CancelStripePaymentIntent;

public record CancelStripePaymentIntentCommand : ICommand<Fin<CancelStripePaymentIntentResult>>
{
    public Guid PaymentId { get; init; }
}

public record CancelStripePaymentIntentResult
{
    public Guid PaymentId { get; init; }
    public string PaymentIntentId { get; init; } = null!;
    public string Status { get; init; } = null!;
    public DateTime CancelledAt { get; init; }
}

internal class CancelStripePaymentIntentCommandHandler(
    PaymentDbContext dbContext,
    IStripePaymentService stripePaymentService,
    IUserContext userContext,
    IClock clock,
    ILogger<CancelStripePaymentIntentCommandHandler> logger)
    : ICommandHandler<CancelStripePaymentIntentCommand, Fin<CancelStripePaymentIntentResult>>
{
    public async Task<Fin<CancelStripePaymentIntentResult>> Handle(
        CancelStripePaymentIntentCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            LogEvents.PaymentProcessed,
            "Cancelling payment intent for Payment {PaymentId}",
            request.PaymentId);

        var cancelledAt = clock.UtcNow;

        var db = GetUpdateEntity<PaymentDbContext, Domain.Models.Payment>(
                p => p.Id == PaymentId.From(request.PaymentId),
                NotFoundError.New($"Payment with id {request.PaymentId} not found."),
                null,
                payment =>
                    from _ in userContext.IsSameUser<IO>(payment.UserId,
                        UnAuthorizedError.New($"You don't have permission to access this payment.")).As()
                    from x in liftIO(async e => await stripePaymentService.CancelPaymentIntentAsync(
                        payment.TransactionId!,
                        e.Token))
                    from result in x.As()
                    from cancelled in payment.MarkAsCancelled(cancelledAt).As()
                    select cancelled)
            .Map(payment => new CancelStripePaymentIntentResult
            {
                PaymentId = payment.Id.Value,
                PaymentIntentId = payment.TransactionId!,
                Status = "cancelled",
                CancelledAt = cancelledAt
            });

        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));

        result.Match(
            _ => logger.LogInformation(
                LogEvents.PaymentCancelled,
                "Payment intent cancelled successfully for Payment {PaymentId}",
                request.PaymentId),
            err => logger.LogError(
                LogEvents.PaymentFailed,
                err,
                "Failed to cancel payment intent for Payment {PaymentId}",
                request.PaymentId));

        return result;
    }
}
