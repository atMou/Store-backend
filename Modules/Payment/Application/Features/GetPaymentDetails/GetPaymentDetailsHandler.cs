using Payment.Application.Contracts;

namespace Payment.Application.Features.GetPaymentDetails;
internal record GetPaymentDetailsCommand : ICommand<Fin<PaymentResult>>
{
	public PaymentId PaymentId { get; init; }

}


internal class GetPaymentDetailsHandler(IClock clock, PaymentDbContext dbContext)
	: ICommandHandler<GetPaymentDetailsCommand, Fin<PaymentResult>>
{
	public Task<Fin<PaymentResult>> Handle(GetPaymentDetailsCommand request, CancellationToken cancellationToken)
	{
		return GetEntity<PaymentDbContext, Domain.Models.Payment>(payment => payment.Id == request.PaymentId,
				NotFoundError.New($"Payment with ID {request.PaymentId} not found.")
			).Map(payment => payment.ToResult())
			.RunAsync(dbContext, EnvIO.New(null, cancellationToken));

	}
}

public record PaymentResult
{
	public Guid PaymentId { get; init; }
	public decimal Total { get; init; }
	public decimal Tax { get; init; }
	public string Status { get; init; }
	public DateTime PaidAt { get; init; }
	public DateTime RefundedAt { get; init; }
	public DateTime FailedAt { get; init; }
	public string PaymentMethod { get; init; }
}