using WebAppRazor.DAL.Models;

namespace WebAppRazor.DAL.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task<bool> CreateAsync(User user);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> UpdateAsync(User user);
    }
}
