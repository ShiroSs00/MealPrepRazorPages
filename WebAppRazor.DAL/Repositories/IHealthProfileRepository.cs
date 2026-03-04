using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public interface IHealthProfileRepository
    {
        Task<HealthProfile?> GetLatestByUserIdAsync(int userId);
        Task<List<HealthProfile>> GetAllByUserIdAsync(int userId);
        Task<bool> CreateAsync(HealthProfile profile);
    }
}
