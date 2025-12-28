
namespace Payment.Application.Features.PaymentStart;

internal record PaymentStartCommand : ICommand<Fin<Unit>>
{
	public OrderId OrderId { get; init; } = null!;
	public UserId UserId { get; init; } = null!;
	public CartId CartId { get; init; } = null!;
	public decimal Tax { get; init; }
	public decimal Total { get; init; }
}
internal class PaymentStartCommandHandler(PaymentDbContext dbContext, IUserContext userContext) : ICommandHandler<PaymentStartCommand, Fin<Unit>>
{
	public Task<Fin<Unit>> Handle(PaymentStartCommand request, CancellationToken cancellationToken)
	{
		var db = from a1 in userContext.IsSameUser<IO>(request.UserId,
			InvalidOperationError.New($"User {request.UserId} is not authorized to initiate payment")).As()
				 from a in AddEntity<PaymentDbContext, Domain.Models.Payment>(
						 Domain.Models.Payment.Create(request.OrderId, request.UserId, request.CartId,
							  request.Total, request.Tax))
				 select unit;

		return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
	}


}
