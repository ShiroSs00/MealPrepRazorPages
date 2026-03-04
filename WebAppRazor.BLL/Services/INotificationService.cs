using WebAppRazor.BLL.DTOs;

namespace WebAppRazor.BLL.Services
{
    public interface INotificationService
    {
        Task<bool> CreateNotificationAsync(int userId, string title, string message, string type);
        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<List<NotificationDto>> GetUnreadNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task CreateMealReminderAsync(int userId, string mealType);
        Task CreateReviewReminderAsync(int userId, string mealName);
        Task CreateShoppingReminderAsync(int userId);
        Task CreateNewMenuNotificationAsync(int userId);
        Task CreateScheduledMealReminderAsync(int userId, string mealType, DateTime scheduledAtUtc);
        Task<List<NotificationDto>> GetDueNotificationsAsync();
        Task MarkNotificationsAsSentAsync(List<int> notificationIds);
    }
}
