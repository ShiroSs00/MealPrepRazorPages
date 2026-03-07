using Microsoft.AspNetCore.SignalR;

namespace WebAppRazor.Web.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        public async Task LeaveUserGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        public async Task JoinMealGroup(int mealItemId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"meal_{mealItemId}");
        }

        public async Task LeaveMealGroup(int mealItemId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"meal_{mealItemId}");
        }

        public static async Task SendNotificationToUser(IHubContext<NotificationHub> hubContext, int userId, string title, string message, string type)
        {
            await hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", title, message, type);
        }

        public static async Task SendUnreadCountToUser(IHubContext<NotificationHub> hubContext, int userId, int count)
        {
            await hubContext.Clients.Group($"user_{userId}").SendAsync("UpdateUnreadCount", count);
        }

        public static async Task BroadcastReview(IHubContext<NotificationHub> hubContext, int mealItemId, object review)
        {
            await hubContext.Clients.Group($"meal_{mealItemId}").SendAsync("ReceiveNewReview", review);
            // Also broadcast to global reviews group for the "Recent Reviews" section
            await hubContext.Clients.All.SendAsync("ReceiveNewGlobalReview", review);
        }
    }
}
