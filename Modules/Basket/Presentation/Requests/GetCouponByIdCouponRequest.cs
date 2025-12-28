namespace Basket.Presentation.Requests;

public record GetCouponByIdRequest
{
	public Guid CouponId { get; init; }

	public GetCouponByIdCommand ToCommand()
	{
		return new GetCouponByIdCommand(Shared.Domain.ValueObjects.CouponId.From(CouponId));
	}
}