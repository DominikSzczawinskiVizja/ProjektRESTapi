//Głowne miejsce gdzie kod rozmawia z bazą danych
using api.Data;
using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Repositories.AuctionRepo
{
    public class AuctionRepository(AppDbContext context) : IAuctionRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<Auction>> GetAllAsync()
        {
            return await _context.Auctions.ToListAsync();
        }
        public async Task<Auction?> GetByIdAsync(long id)
        {
            return await _context.Auctions.FindAsync(id);
        }
        public async Task<Auction> AddAuctionAsync(Auction auction)
        {
            await _context.Auctions.AddAsync(auction);
            await _context.SaveChangesAsync();
            return auction;
        }
        public async Task<Auction> UpdateAuctionAsync(Auction auction)
        {
            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();
            return auction;
        }
        public async Task DeleteAuctionAsync(long id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            _context.Auctions.Remove(auction!);
            await _context.SaveChangesAsync();
        }
    }
}
