namespace WebAppRazor.BLL.Services
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class RegisterResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(string username, string password);
        Task<RegisterResult> RegisterAsync(string username, string password, string fullName, string email);
    }
}
