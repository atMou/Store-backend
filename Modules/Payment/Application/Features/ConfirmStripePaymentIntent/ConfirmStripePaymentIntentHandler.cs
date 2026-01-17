using Microsoft.Extensions.Logging;

using Order.Domain.ValueObjects;

using Payment.Domain.ValueObjects;
using Payment.Infrastructure.Stripe;

using Shared.Infrastructure.Errors;
using Shared.Infrastructure.Logging;

namespace Payment.Application.Features.ConfirmStripePaymentIntent;

public record ConfirmStripePaymentIntentCommand : ICommand<Fin<ConfirmStripePaymentIntentResult>>
{
    public Guid PaymentId { get; init; }
}

public record ConfirmStripePaymentIntentResult
{
    public Guid PaymentId { get; init; }
    public string PaymentIntentId { get; init; } = null!;
    public string Status { get; init; } = null!;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;
}

internal class ConfirmStripePaymentIntentCommandHandler(
    PaymentDbContext dbContext,
    IStripePaymentService stripePaymentService,
    IUserContext userContext,
    IClock clock,
    ILogger<ConfirmStripePaymentIntentCommandHandler> logger)
    : ICommandHandler<ConfirmStripePaymentIntentCommand, Fin<ConfirmStripePaymentIntentResult>>
{
    public async Task<Fin<ConfirmStripePaymentIntentResult>> Handle(
        ConfirmStripePaymentIntentCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            LogEvents.PaymentProcessed,
            "Processing payment confirmation for Payment {PaymentId}",
            request.PaymentId);

        var db = GetUpdateEntity<PaymentDbContext, Domain.Models.Payment>(
                p => p.Id == PaymentId.From(request.PaymentId),
                NotFoundError.New($"Payment with id {request.PaymentId} not found."),
                null,
                payment =>
                    from _ in userContext.IsSameUser<IO>(payment.UserId,
                        UnAuthorizedError.New($"You don't have permission to access this payment.")).As()
                    from stripeResult in liftIO(async e => await stripePaymentService.GetPaymentIntentAsync(
                        payment.TransactionId!,
                        e.Token))
                    from response in stripeResult.Bind(
                        r => (r.Status == "succeeded" && payment.PaymentStatus != PaymentStatus.Paid)
                            ? payment.MarkAsFulfilled(PaymentMethod.Stripe, clock.UtcNow).Map(_ => payment)
                            : FinSucc(payment))
                    select response)
            .Map(payment => new ConfirmStripePaymentIntentResult
            {
                PaymentId = payment.Id.Value,
                PaymentIntentId = payment.TransactionId!,
                Status = payment.PaymentStatus.Name.ToLowerInvariant(),
                Amount = payment.Total,
                Currency = "eur"
            });

        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));

        result.Match(
            r => logger.LogInformation(
                LogEvents.PaymentCompleted,
                "Payment confirmation processed for Payment {PaymentId} with status {Status}",
                request.PaymentId, r.Status),
            err => logger.LogError(
                LogEvents.PaymentFailed,
                err,
                "Failed to process payment confirmation for Payment {PaymentId}",
                request.PaymentId));

        return result;
    }
}
