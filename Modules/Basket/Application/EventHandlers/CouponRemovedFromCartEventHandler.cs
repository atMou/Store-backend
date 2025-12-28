using Basket.Application.Features.Coupon.UpdateCoupon;
using Basket.Domain.Events;

using Microsoft.Extensions.Logging;

namespace Basket.Application.EventHandlers;

public record CouponRemovedFromCartEventHandler(ISender Sender, ILogger<CouponRemovedFromCartEventHandler> Logger) : INotificationHandler<CouponRemovedFromCartDomainEvent>
{
	public async Task Handle(CouponRemovedFromCartDomainEvent notification, CancellationToken cancellationToken)
	{
		var result =
			await Sender.Send(new UpdateCouponCommand(new UpdateCouponDto()
			{ Status = CouponStatus.AssignedToUser }), cancellationToken); ;
		result.MapFail(err =>
		{
			Logger.LogError("Failed to update coupon status for CouponId {CouponId}: {err}",
				notification.CouponId.Value, err.Message);
			return err;
		});

	}
}