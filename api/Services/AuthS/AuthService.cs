using api.DTOs.AuthDto;
using api.Models;
using api.Repositories.UserRepo;
using Microsoft.AspNetCore.Identity;
namespace api.Services.AuthS
{
    public class AuthService(IUserRepository users, ITokenService tokens, IPasswordHasher<User> passwordHasher) : IAuthService
    {
        private readonly IUserRepository _users = users;
        private readonly ITokenService _tokens = tokens;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existing = await _users.GetByEmailAsync(dto.Email);
            if (existing != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }
            var user = new User
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CreatedAt = DateTime.UtcNow,
                Role = "User",
                PasswordHash = ""
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            var created = await _users.AddUserAsync(user);

            var token = _tokens.CreateAccessToken(created);

            return new AuthResponseDto
            {
                AccessToken = token,
                UserId = created.Id,
                Email = dto.Email,
                Role = created.Role,
            };
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email);
            if(user == null)
            {
                throw new UnauthorizedAccessException("Invalid E-mail or Password.");
            }
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if(result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid E-mail or Password");
            }

            var token = _tokens.CreateAccessToken(user);

            return new AuthResponseDto
            {
                AccessToken = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
            };
        }
    }


}
