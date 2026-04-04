using api.Models;
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
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<User> AddUserAsync(User user)
        {
            return await _repository.AddUserAsync(user);
        }
        public async Task<User> UpdateUserAsync(User user)
        {
            return await _repository.UpdateUserAsync(user);
        }
        public async Task DeleteUserAsync(int id)
        {
            await _repository.DeleteUserAsync(id);
        }
    }
}
