using Payment.Infrastructure.Stripe;

using Shared.Infrastructure.Errors;

namespace Payment.Application.Features.CreateStripePaymentIntent;

public record CreateStripePaymentIntentCommand : ICommand<Fin<CreateStripePaymentIntentResult>>
{
    public CartId CartId { get; init; }
}

public record CreateStripePaymentIntentResult
{
    public Guid PaymentId { get; init; }
    public string ClientSecret { get; init; } = null!;
    public string PaymentIntentId { get; init; } = null!;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;
}

internal class CreateStripePaymentIntentCommandHandler(
    PaymentDbContext dbContext,
    IStripePaymentService stripePaymentService,
    IConfiguration configuration,
    IUserContext userContext)
    : ICommandHandler<CreateStripePaymentIntentCommand, Fin<CreateStripePaymentIntentResult>>
{
    public async Task<Fin<CreateStripePaymentIntentResult>> Handle(
        CreateStripePaymentIntentCommand request,
        CancellationToken cancellationToken)
    {
        var currency = configuration["Stripe:Currency"] ?? "eur";

        var db = GetUpdateEntity<PaymentDbContext, Domain.Models.Payment>(
                p => p.CartId == request.CartId,
                NotFoundError.New($"Payment for cart with id {request.CartId.Value} not found."),
                null,
                //payment => payment.EnsureCanMakePayment(PaymentMethod.Stripe).As(),
                payment =>
                    from user in userContext.GetCurrentUser<IO>()
                    from _ in userContext.IsSameUser<IO>(payment.UserId,
                        UnAuthorizedError.New($"You don't have permission to access this payment."))
                    from x in liftIO(async e => await stripePaymentService.CreatePaymentIntentAsync(
                        payment.Total,
                        currency,
                        payment.OrderId.Value,
                        payment.UserId.Value,
                        user.Email,
                        e.Token))
                    from result in x.As()
                    select payment.SetStripePaymentIntentId(result.PaymentIntentId, result.Amount, result.Currency,
                        result.ClientSecret))
            .Map(payment => new CreateStripePaymentIntentResult
            {
                PaymentId = payment.Id.Value,
                ClientSecret = payment.ClientSecret!,
                PaymentIntentId = payment.TransactionId!,
                Amount = payment.Amount,
                Currency = payment.Currency!
            });


        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));


    }
}