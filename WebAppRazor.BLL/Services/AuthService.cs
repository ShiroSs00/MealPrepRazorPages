using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using WebAppRazor.DAL.Models;
using WebAppRazor.DAL.Repositories;

namespace WebAppRazor.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng."
                };
            }

            if (!VerifyPassword(password, user.PasswordHash))
            {
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng."
                };
            }

            return new LoginResult
            {
                Success = true,
                UserId = user.Id,
                Username = user.Username,
                FullName = user.FullName
            };
        }

        public async Task<RegisterResult> RegisterAsync(string username, string password, string fullName, string email)
        {
            if (await _userRepository.UsernameExistsAsync(username))
            {
                return new RegisterResult
                {
                    Success = false,
                    ErrorMessage = "Tên đăng nhập đã tồn tại."
                };
            }

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                FullName = fullName,
                Email = email,
                CreatedAt = DateTime.Now
            };

            var created = await _userRepository.CreateAsync(user);
            if (!created)
            {
                return new RegisterResult
                {
                    Success = false,
                    ErrorMessage = "Không thể tạo tài khoản. Vui lòng thử lại."
                };
            }

            return new RegisterResult { Success = true };
        }

        private static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32));

            return hashed == parts[1];
        }
    }
}
