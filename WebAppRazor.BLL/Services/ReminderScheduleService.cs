using WebAppRazor.BLL.DTOs;
using WebAppRazor.DAL.Models;
using WebAppRazor.DAL.Repositories;

namespace WebAppRazor.BLL.Services
{
    public class ReminderScheduleService : IReminderScheduleService
    {
        private readonly IReminderScheduleRepository _repository;

        public ReminderScheduleService(IReminderScheduleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ReminderScheduleDto>> GetUserSchedulesAsync(int userId)
        {
            var schedules = await _repository.GetByUserIdAsync(userId);
            return schedules.Select(MapToDto).ToList();
        }

        public async Task<bool> CreateScheduleAsync(int userId, string reminderType, TimeOnly reminderTime,
            DateOnly startDate, DateOnly? endDate, string repeatMode)
        {
            var schedule = new ReminderSchedule
            {
                UserId = userId,
                ReminderType = reminderType,
                ReminderTime = reminderTime,
                StartDate = startDate,
                EndDate = endDate,
                RepeatMode = repeatMode,
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            return await _repository.CreateAsync(schedule);
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId, int userId)
        {
            var schedule = await _repository.GetByIdAsync(scheduleId);
            if (schedule == null || schedule.UserId != userId) return false;
            return await _repository.DeleteAsync(scheduleId);
        }

        public async Task<bool> ToggleActiveAsync(int scheduleId, int userId)
        {
            var schedule = await _repository.GetByIdAsync(scheduleId);
            if (schedule == null || schedule.UserId != userId) return false;
            schedule.IsActive = !schedule.IsActive;
            return await _repository.UpdateAsync(schedule);
        }

        public async Task<List<ReminderScheduleDto>> GetAllActiveSchedulesAsync()
        {
            var schedules = await _repository.GetAllActiveAsync();
            return schedules.Select(MapToDto).ToList();
        }

        public async Task<bool> UpdateLastTriggeredAsync(int scheduleId, DateTime triggeredAt)
        {
            var schedule = await _repository.GetByIdAsync(scheduleId);
            if (schedule == null) return false;
            schedule.LastTriggeredAt = triggeredAt;
            return await _repository.UpdateAsync(schedule);
        }

        private static ReminderScheduleDto MapToDto(ReminderSchedule entity)
        {
            return new ReminderScheduleDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                ReminderType = entity.ReminderType,
                ReminderTime = entity.ReminderTime,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                RepeatMode = entity.RepeatMode,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                LastTriggeredAt = entity.LastTriggeredAt
            };
        }
    }
}
