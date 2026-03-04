using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdAsync(int userId);
        Task<List<Notification>> GetUnreadByUserIdAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> CreateAsync(Notification notification);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<List<Notification>> GetDueNotificationsAsync();
        Task<bool> MarkAsSentAsync(List<int> notificationIds);
    }
}
