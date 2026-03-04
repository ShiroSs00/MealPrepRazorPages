using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public interface IProgressRepository
    {
        Task<List<ProgressEntry>> GetByUserIdAsync(int userId);
        Task<ProgressEntry?> GetLatestByUserIdAsync(int userId);
        Task<bool> CreateAsync(ProgressEntry entry);
    }
}
