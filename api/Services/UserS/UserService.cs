using api.Models;
using api.DTOs.UserDto;
using api.Repositories.UserRepo;

namespace api.Services.UserS
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<User?> GetByIdAsync(long id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID: {id} not found");
            }
            return user;
        }
        public async Task<User> AddUserAsync(UserCreateDto dto)
        {
            var newUser = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.Password,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.AddUserAsync(newUser);
        }
        public async Task<User> UpdateUserAsync(long id, UserUpdateDto dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID: {id} not found");
            }

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Email = dto.Email ?? user.Email;

            return await _repository.UpdateUserAsync(user);
        }
        public async Task DeleteUserAsync(long id)
        {
            await _repository.DeleteUserAsync(id);
        }
    }
}
