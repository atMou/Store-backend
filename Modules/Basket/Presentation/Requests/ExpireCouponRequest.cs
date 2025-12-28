namespace Basket.Presentation.Requests;

public record ExpireCouponRequest(Guid CouponId)
{
	public ExpireCouponCommand ToCommand()
	{
		return new ExpireCouponCommand(Shared.Domain.ValueObjects.CouponId.From(CouponId));
	}
}