using System.Security.Claims;

using MassTransit;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using Shared.Infrastructure.Hubs.Services;

namespace Shared.Infrastructure.Hubs;

[Authorize]
public class NotificationHub(
    ILogger<NotificationHub> logger,
    IPublishEndpoint endpoint,
    ISender sender,
    ISignalRSubscriptionStore subscriptionStore
) : Hub<INotification>
{

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

            var roles = Context.User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];
            foreach (var role in roles)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"role_{role}");
            }

            var permissions = Context.User?.FindAll(Claims.Permissions).Select(c => c.Value) ?? [];
            foreach (var p in permissions)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"permission_{p}");
            }


            try
            {
                var subscriptions = await subscriptionStore.GetUserSubscriptionsAsync(userId);

                foreach (var groupName in subscriptions)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                }

                logger.LogInformation(
                    "Restored {Count} subscriptions from Redis for user {UserId}",
                    subscriptions.Count(),
                    userId);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to restore subscriptions from Redis for user {UserId}",
                    userId);
            }


        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");

            var roles = Context.User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];
            foreach (var role in roles)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"role_{role}");
            }

            var permissions = Context.User?.FindAll(Claims.Permissions).Select(c => c.Value) ?? [];
            foreach (var p in permissions)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"permission_{p}");
            }

            // ✅ Don't clear subscriptions on disconnect - they persist across reconnections
            logger.LogInformation(
                "User {UserId} disconnected. Subscriptions preserved in Redis for reconnection.",
                userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SubscribeToOrder(string orderId)
    {

        var groupName = $"order_{orderId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                await subscriptionStore.SubscribeAsync(userId, groupName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to persist subscription to Redis for order {OrderId}", orderId);
            }
        }
    }

    public async Task SubscribeToShipment(string shipmentId)
    {
        var groupName = $"shipment_{shipmentId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                await subscriptionStore.SubscribeAsync(userId, groupName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to persist subscription to Redis for shipment {ShipmentId}", shipmentId);
            }
        }
    }

    public async Task SubscribeToProduct(string productId, string colorCode, string sizeCode)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        logger.LogInformation("⏱️ START SubscribeToProduct - {ProductId}/{Color}/{Size}", productId, colorCode,
            sizeCode);

        var groupName = $"product_{productId}_{colorCode}_{sizeCode}";
        logger.LogInformation(
            "👤 User subscribing to group: {GroupName} | ProductId: {ProductId}, Color: {Color}, Size: {Size}, ConnectionId: {ConnectionId}",
            groupName,
            productId,
            colorCode,
            sizeCode,
            Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        logger.LogInformation("⏱️ Added to group: {ElapsedMs}ms", sw.ElapsedMilliseconds);

        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                await subscriptionStore.SubscribeAsync(userId, groupName);
                logger.LogInformation("⏱️ Persisted subscription to Redis: {ElapsedMs}ms", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to persist subscription to Redis for product {ProductId} color {Color} size {Size}",
                    productId, colorCode, sizeCode);
            }
        }

        logger.LogInformation("⏱️ COMPLETE SubscribeToProduct: {TotalMs}ms", sw.ElapsedMilliseconds);
    }

    public async Task UnsubscribeFromProduct(string productId, string colorCode, string sizeCode)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        logger.LogInformation("⏱️ START UNSubscribeToProduct - {ProductId}/{Color}/{Size}", productId, colorCode,
            sizeCode);

        var groupName = $"product_{productId}_{colorCode}_{sizeCode}";

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                await subscriptionStore.UnsubscribeAsync(userId, groupName);
                logger.LogInformation("⏱️ Removed subscription from Redis: {ElapsedMs}ms", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to remove subscription from Redis for product {ProductId} color {Color} size {Size}",
                    productId, colorCode, sizeCode);
            }
        }

        logger.LogInformation("⏱️ COMPLETE UNSubscribeToProduct: {TotalMs}ms", sw.ElapsedMilliseconds);
    }

    public async Task SignOut()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                await subscriptionStore.ClearUserSubscriptionsAsync(userId);
                logger.LogInformation(
                    "Cleared all subscriptions for user {UserId} on explicit sign out",
                    userId);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to clear subscriptions on sign out for user {UserId}",
                    userId);
            }
        }
    }

    //private IO<Guid> parseUserId(string userId) => IO.pure(Guid.Parse(userId));

    //private IO<UserResult> getUserById(Guid userId) =>
    //    liftIO(async e =>
    //        await sender.Send(new GetUserByIdQuery(UserId.From(userId)), e.Token))
    //    .Bind(fin => fin.Match(
    //       IO.pure,
    //         IO.fail<UserResult>
    //    ));

    //private IO<Unit> restoreUserSubscriptions(UserResult user, string userId) =>
    //    liftIO(async () =>
    //    {
    //        foreach (var subscription in user.ProductSubscriptions)
    //        {
    //            await Groups.AddToGroupAsync(Context.ConnectionId, subscription);
    //        }
    //        logger.LogInformation("Restored {Count} subscriptions for user {UserId}",
    //            user.ProductSubscriptions.Count(), userId);
    //        return unit;
    //    });

}


