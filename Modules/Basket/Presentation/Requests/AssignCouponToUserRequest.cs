using Basket.Application.Features.Coupon.AssignCouponToUser;

namespace Basket.Presentation.Requests;

public record AssignCouponToUserRequest
{
	public Guid UserId { get; init; }
	public Guid CouponId { get; init; }

	public AssignCouponToUserCommand ToCommand()
	{
		return new AssignCouponToUserCommand(Shared.Domain.ValueObjects.UserId.From(UserId),
			Shared.Domain.ValueObjects.CouponId.From(CouponId));
	}

}