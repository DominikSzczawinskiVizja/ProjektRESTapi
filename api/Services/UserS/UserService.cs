//Miejsce w którym program waliduje dane
using api.DTOs.UserDto;
using api.Models;
using api.Repositories.AuctionRepo;
using api.Repositories.UserRepo;

using Microsoft.AspNetCore.Identity;

namespace api.Services.UserS
{
    public class UserService(
        IUserRepository repository,
        IAuctionRepository auction,
        IPasswordHasher<User> passwordHasher
        ) : IUserService
    {
        private readonly IUserRepository _repository = repository;
        private readonly IAuctionRepository _auction = auction;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<User> GetByIdAsync(long id)
        {
            return await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"User with ID: {id} not found");
        }
        public async Task<User> AddUserAsync(UserCreateDto dto)
        {   //dane wysylane przez klienta
            var newUser = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = "",
                CreatedAt = DateTime.UtcNow
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, dto.Password);

            return await _repository.AddUserAsync(newUser);
        }
        public async Task<User> UpdateUserAsync(long id, long currentUserId, UserUpdateDto dto)
        {
            if (id != currentUserId)
                throw new UnauthorizedAccessException("You can only update your own account");

            var user = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"User with ID: {id} not found");

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Email = dto.Email ?? user.Email;

            return await _repository.UpdateUserAsync(user);
        }
        public async Task DeleteUserAsync(long id, long currentUserId)
        {
            if (id != currentUserId)
                throw new UnauthorizedAccessException("You can only delete your own account");

            var user = await GetByIdAsync(id);

            if (user.Auctions.Count != 0) 
                throw new InvalidOperationException("Cannot delete Account when auction is ongoing");

            await _repository.DeleteUserAsync(id);
        }
    }
}
