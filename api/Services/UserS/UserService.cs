//Miejsce w którym program waliduje dane
using api.Models;
using api.DTOs.UserDto;
using api.Repositories.UserRepo;
using api.Repositories.AuctionRepo;

namespace api.Services.UserS
{
    public class UserService(IUserRepository repository, IAuctionRepository auction) : IUserService
    {
        private readonly IUserRepository _repository = repository;
        private readonly IAuctionRepository _auction = auction;

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<User?> GetByIdAsync(long id)
        {
            var user = await _repository.GetByIdAsync(id);
            return user ?? throw new KeyNotFoundException($"User with ID: {id} not found");
        }
        public async Task<User> AddUserAsync(UserCreateDto dto)
        {   //dane wysylane przez klienta
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
            user!.FirstName = dto.FirstName ?? user.FirstName;
            user!.LastName = dto.LastName ?? user.LastName;
            user!.Email = dto.Email ?? user.Email;

            return await _repository.UpdateUserAsync(user);
        }
        public async Task DeleteUserAsync(long id)
        {
            var user = await GetByIdAsync(id);
            if(user!.Auctions.Count != 0)
            {
                throw new InvalidOperationException("Cannot delete Account when auction is ongoing");
            }
            await _repository.DeleteUserAsync(id);
        }
    }
}
