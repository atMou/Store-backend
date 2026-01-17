using Order.Domain.ValueObjects;

using Payment.Infrastructure.Stripe;

using Shared.Infrastructure.Errors;

namespace Payment.Application.Features.ProcessStripeRefund;

public record ProcessStripeRefundCommand : ICommand<Fin<ProcessStripeRefundResult>>
{
    public PaymentId PaymentId { get; init; }
    public decimal? RefundAmount { get; init; }
}

public record ProcessStripeRefundResult
{
    public Guid PaymentId { get; init; }
    public string RefundId { get; init; } = null!;
    public decimal RefundedAmount { get; init; }
    public string Status { get; init; } = null!;
}

internal class ProcessStripeRefundCommandHandler(
    PaymentDbContext dbContext,
    IStripePaymentService stripePaymentService,
    IUserContext userContext,
    IClock clock)
    : ICommandHandler<ProcessStripeRefundCommand, Fin<ProcessStripeRefundResult>>
{
    public async Task<Fin<ProcessStripeRefundResult>> Handle(
        ProcessStripeRefundCommand request,
        CancellationToken cancellationToken)
    {
        var db =
            from _ in userContext.HasPermission<IO>(Permission.MakeRefund,
                UnAuthorizedError.New($"You are not authorized to make refunds.")).As()
            from p in GetUpdateEntity<PaymentDbContext, Domain.Models.Payment>(
                    p => p.Id == request.PaymentId,
                    NotFoundError.New($"Payment with ID {request.PaymentId} not found."),
                    null,
                    payment =>
                        from x in liftIO(async e => await stripePaymentService.RefundPaymentAsync(
                            payment.TransactionId!,
                            request.RefundAmount,
                            e.Token))
                        from result in x.As()
                        from p in payment.MarkAsRefunded(PaymentMethod.Stripe, clock.UtcNow, result.Amount,
                            result.Status, result.RefundId).As()
                        select p)
                .Map(payment => new ProcessStripeRefundResult
                {
                    PaymentId = payment.Id.Value,
                    RefundId = payment.RefundId!,
                    RefundedAmount = payment.RefundAmount ?? 0,
                    Status = "succeeded"
                })
            select p;

        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
