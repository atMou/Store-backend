using Order.Domain.ValueObjects;

namespace Payment.Application.Features.PaymentCancelled;
internal record PaymentCancelledCommand : ICommand<Fin<Unit>>
{
    public PaymentId PaymentId { get; init; }
    public string PaymentMethod { get; init; } = null!;
}

internal class PaymentCancelledCommandHandler(IClock clock, PaymentDbContext dbContext) : ICommandHandler<PaymentCancelledCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(PaymentCancelledCommand request, CancellationToken cancellationToken)
    {
        return GetUpdateEntity<PaymentDbContext, Domain.Models.Payment>(
                payment => payment.Id == request.PaymentId,
                NotFoundError.New($"Payment with ID {request.PaymentId} not found."),
                null,
                payment =>
                    from paymentMethod in Order.Domain.ValueObjects.PaymentMethod.Stripe.From(request.PaymentMethod).As()
                    from failed in payment.MarkAsFailed(paymentMethod, clock.UtcNow).As()
                    select failed
            ).Map(_ => unit)
            .RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}

