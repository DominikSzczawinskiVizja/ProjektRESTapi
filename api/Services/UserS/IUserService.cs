using api.DTOs.UserDto;
using api.Models;

namespace api.Services.UserS
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(long id);
        Task<User> AddUserAsync(UserCreateDto dto);
        Task<User> UpdateUserAsync(long id, long CurrentUserId, UserUpdateDto dto);
        Task DeleteUserAsync(long id, long CurrentUserId);
    }
}
