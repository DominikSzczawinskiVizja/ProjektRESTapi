//Głowne miejsce gdzie kod rozmawia z bazą danych
using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;


namespace api.Repositories.UserRepo
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User?> GetByIdAsync(long id)
        {
            return await _context.Users
            .Include(u => u.Auctions)
            .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
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
        public async Task DeleteUserAsync(long id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user!);
            await _context.SaveChangesAsync();
        }
    }
}
