using WebAppRazor.BLL.DTOs;
using WebAppRazor.DAL.Models;
using WebAppRazor.DAL.Repositories;

namespace WebAppRazor.BLL.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<bool> CreateNotificationAsync(int userId, string title, string message, string type)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            return await _notificationRepository.CreateAsync(notification);
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId);
            return notifications.Select(MapToDto).ToList();
        }

        public async Task<List<NotificationDto>> GetUnreadNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId);
            return notifications.Select(MapToDto).ToList();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _notificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            return await _notificationRepository.MarkAsReadAsync(notificationId);
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            return await _notificationRepository.MarkAllAsReadAsync(userId);
        }

        public async Task CreateScheduledMealReminderAsync(int userId, string mealType, DateTime scheduledAtUtc)
        {
            string title = mealType switch
            {
                "Breakfast" => "Nhắc nhở bữa sáng",
                "Lunch" => "Nhắc nhở bữa trưa",
                "Dinner" => "Nhắc nhở bữa tối",
                "Snack" => "Nhắc nhở bữa phụ",
                "Shopping" => "Nhắc mua nguyên liệu",
                _ => "Nhắc nhở bữa ăn"
            };

            string message = mealType switch
            {
                "Shopping" => "Đã đến giờ mua nguyên liệu cho thực đơn của bạn!",
                _ => $"Đã đến giờ {title.ToLower()}! Hãy kiểm tra thực đơn của bạn."
            };

            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = mealType == "Shopping" ? "ShoppingReminder" : "MealReminder",
                IsRead = false,
                CreatedAt = scheduledAtUtc,
                ScheduledAt = scheduledAtUtc,
                IsSent = false
            };

            await _notificationRepository.CreateAsync(notification);
        }

        public async Task CreateMealReminderAsync(int userId, string mealType)
        {
            string title = mealType switch
            {
                "Breakfast" => "Nhắc nhở bữa sáng",
                "Lunch" => "Nhắc nhở bữa trưa",
                "Dinner" => "Nhắc nhở bữa tối",
                "Snack" => "Nhắc nhở bữa phụ",
                _ => "Nhắc nhở bữa ăn"
            };

            await CreateNotificationAsync(userId, title,
                $"Đã đến giờ {title.ToLower()}! Hãy kiểm tra thực đơn của bạn.",
                "MealReminder");
        }

        public async Task CreateReviewReminderAsync(int userId, string mealName)
        {
            await CreateNotificationAsync(userId,
                "Nhắc đánh giá món ăn",
                $"Bạn đã dùng \"{mealName}\" chưa? Hãy đánh giá để giúp cải thiện thực đơn!",
                "ReviewReminder");
        }

        public async Task CreateShoppingReminderAsync(int userId)
        {
            await CreateNotificationAsync(userId,
                "Nhắc mua nguyên liệu",
                "Hãy kiểm tra danh sách nguyên liệu cần mua cho thực đơn tuần này!",
                "ShoppingReminder");
        }

        public async Task CreateNewMenuNotificationAsync(int userId)
        {
            await CreateNotificationAsync(userId,
                "Thực đơn mới đã sẵn sàng!",
                "Thực đơn cá nhân hóa của bạn đã được tạo. Hãy xem ngay!",
                "NewMenu");
        }

        public async Task<List<NotificationDto>> GetDueNotificationsAsync()
        {
            var notifications = await _notificationRepository.GetDueNotificationsAsync();
            return notifications.Select(MapToDto).ToList();
        }

        public async Task MarkNotificationsAsSentAsync(List<int> notificationIds)
        {
            await _notificationRepository.MarkAsSentAsync(notificationIds);
        }

        private static NotificationDto MapToDto(Notification entity)
        {
            return new NotificationDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Title = entity.Title,
                Message = entity.Message,
                Type = entity.Type,
                IsRead = entity.IsRead,
                CreatedAt = entity.CreatedAt,
                ScheduledAt = entity.ScheduledAt,
                IsSent = entity.IsSent
            };
        }
    }
}
