using api.Models;

namespace api.Repositories.UserRepo
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User?> GetByIdAsync(long id);

        Task<User> AddUserAsync(User user);

        Task<User> UpdateUserAsync(User user);

        Task DeleteUserAsync(long id);
    }
}
