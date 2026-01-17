using Payment.Application.Contracts;

using Shared.Application.Contracts.Payment.Results;

namespace Payment.Application.Features.GetPaymentByCartIdDetails;
internal record GetPaymentByCartIdDetailsCommand : ICommand<Fin<PaymentResult>>
{
    public CartId CartId { get; init; }

}


internal class GetPaymentByCartIdDetailsHandler(IClock clock, PaymentDbContext dbContext)
    : ICommandHandler<GetPaymentByCartIdDetailsCommand, Fin<PaymentResult>>
{
    public Task<Fin<PaymentResult>> Handle(GetPaymentByCartIdDetailsCommand request, CancellationToken cancellationToken)
    {
        return GetEntity<PaymentDbContext, Domain.Models.Payment>(payment => payment.CartId == request.CartId,
                NotFoundError.New($"Payment with Cart ID {request.CartId.Value} not found.")
            ).Map(payment => payment.ToResult())
            .RunAsync(dbContext, EnvIO.New(null, cancellationToken));

    }
}

