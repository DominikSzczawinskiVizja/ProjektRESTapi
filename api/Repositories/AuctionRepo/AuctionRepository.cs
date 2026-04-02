using api.Data;
using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Repositories.AuctionRepo
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly AppDbContext _context;

        public AuctionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Auction>> GetAllAsync()
        {
            return await _context.Auctions.ToListAsync();
        }
        public async Task<Auction?> GetByIdAsync(int id)
        {
            return await _context.Auctions.FindAsync(id);
        }
        public async Task AddAuctionAsync(Auction auction)
        {
            await _context.Auctions.AddAsync(auction);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAuctionAsync(Auction auction)
        {
            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAuctionAsync(int id)
        {
            var Auction = await _context.Auctions.FindAsync(id);
            if(Auction == null)
            {
                throw new KeyNotFoundException($"Aukcja o id: {id} nie istnieje.");
            }
            else
            {
                _context.Auctions.Remove(Auction);
                await _context.SaveChangesAsync();
            }
        }
    }
}
