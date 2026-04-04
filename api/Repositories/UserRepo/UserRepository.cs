using api.Data;
using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Repositories.UserRepo
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<User> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task DeleteUserAsync(int id)
        {
            var User = await _context.Users.FindAsync(id);
            if(User == null)
            {
                throw new KeyNotFoundException($"Nie znaleziono uzytkownika o id: {id}");
            }
            else
            {
                _context.Users.Remove(User);
                await _context.SaveChangesAsync();
            }
        }
    }
}
