using System.Threading.Tasks;
using WebAppRazor.BLL.DTOs;

namespace WebAppRazor.BLL.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync(int userId, string filterType);
        Task SeedMockDataAsync(int userId);
    }
}
