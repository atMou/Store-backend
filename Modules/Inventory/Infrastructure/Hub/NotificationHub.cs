using System.Security.Claims;

using Microsoft.AspNetCore.SignalR;

namespace Inventory.Infrastructure.Hub;
public class NotificationHub : Hub<INotificationClient>
{
	public override async Task OnConnectedAsync()
	{
		var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
					 ?? Context.UserIdentifier;

		if (!string.IsNullOrEmpty(userId))
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, userId);
		}

		await base.OnConnectedAsync();
	}
}


