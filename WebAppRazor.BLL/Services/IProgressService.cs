using WebAppRazor.BLL.DTOs;

namespace WebAppRazor.BLL.Services
{
    public interface IProgressService
    {
        Task<bool> LogProgressAsync(int userId, double weight, string notes);
        Task<List<ProgressEntryDto>> GetProgressHistoryAsync(int userId);
        Task<ProgressEntryDto?> GetLatestProgressAsync(int userId);
    }
}
