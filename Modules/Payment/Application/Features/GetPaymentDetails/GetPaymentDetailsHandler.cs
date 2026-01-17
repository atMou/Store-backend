using Payment.Application.Contracts;

using Shared.Application.Contracts.Payment.Results;

namespace Payment.Application.Features.GetPaymentDetails;
internal record GetPaymentDetailsCommand : ICommand<Fin<PaymentResult>>
{
    public PaymentId PaymentId { get; init; }

}


internal class GetPaymentByCartIdDetailsHandler(IClock clock, PaymentDbContext dbContext)
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

