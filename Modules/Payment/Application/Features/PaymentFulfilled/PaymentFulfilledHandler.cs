namespace Payment.Application.Features.PaymentFulfilled;
internal record PaymentFulfilledCommand : ICommand<Fin<Unit>>
{
	public PaymentId PaymentId { get; init; }
	public string TransactionId { get; init; }
	public string PaymentMethod { get; init; }
}

internal class PaymentFulfilledCommandHandler(IClock clock, PaymentDbContext dbContext) : ICommandHandler<PaymentFulfilledCommand, Fin<Unit>>
{
	public Task<Fin<Unit>> Handle(PaymentFulfilledCommand request, CancellationToken cancellationToken)
	{
		return GetUpdateEntity<PaymentDbContext, Domain.Models.Payment>(payment => payment.Id == request.PaymentId,
				NotFoundError.New($"Payment with ID {request.PaymentId} not found."),
				null,
				payment => payment.MarkAsFulfilled(request.PaymentMethod, request.TransactionId, clock.UtcNow)

			).Map(_ => unit)
			.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
	}
}

