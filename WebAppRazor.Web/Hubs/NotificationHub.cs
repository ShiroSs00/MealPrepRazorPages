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

        public static async Task SendNotificationToUser(IHubContext<NotificationHub> hubContext, int userId, string title, string message, string type)
        {
            await hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", title, message, type);
        }

        public static async Task SendUnreadCountToUser(IHubContext<NotificationHub> hubContext, int userId, int count)
        {
            await hubContext.Clients.Group($"user_{userId}").SendAsync("UpdateUnreadCount", count);
        }
    }
}
