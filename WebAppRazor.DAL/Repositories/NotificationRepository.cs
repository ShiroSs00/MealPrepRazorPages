using Microsoft.EntityFrameworkCore;
using WebAppRazor.DAL.Data;
using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<bool> CreateAsync(Notification notification)
        {
            try
            {
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(notificationId);
                if (notification == null) return false;
                notification.IsRead = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();

                foreach (var n in notifications)
                {
                    n.IsRead = true;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Notification>> GetDueNotificationsAsync()
        {
            var nowUtc = DateTime.UtcNow;
            return await _context.Notifications
                .Where(n => !n.IsSent && n.ScheduledAt != null && n.ScheduledAt <= nowUtc)
                .ToListAsync();
        }

        public async Task<bool> MarkAsSentAsync(List<int> notificationIds)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => notificationIds.Contains(n.Id))
                    .ToListAsync();

                foreach (var n in notifications)
                {
                    n.IsSent = true;
                    n.CreatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
