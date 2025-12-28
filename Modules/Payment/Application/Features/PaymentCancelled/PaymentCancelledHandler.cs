namespace Payment.Application.Features.PaymentCancelled;
internal record PaymentCancelledCommand : ICommand<Fin<Unit>>
{
	public PaymentId PaymentId { get; init; }
	public string TransactionId { get; init; }
	public string PaymentMethod { get; init; }
}

internal class PaymentCancelledCommandHandler(IClock clock, PaymentDbContext dbContext) : ICommandHandler<PaymentCancelledCommand, Fin<Unit>>
{
	public Task<Fin<Unit>> Handle(PaymentCancelledCommand request, CancellationToken cancellationToken)
	{
		return GetUpdateEntity<PaymentDbContext, Domain.Models.Payment>(payment => payment.Id == request.PaymentId,
				NotFoundError.New($"Payment with ID {request.PaymentId} not found."),
				null,
				payment => payment.MarkAsFailed(request.PaymentMethod, clock.UtcNow)

			).Map(_ => unit)
			.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
	}
}

