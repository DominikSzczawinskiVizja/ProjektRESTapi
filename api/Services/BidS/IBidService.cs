using api.Models;

namespace api.Services.BidS
{
    public interface IBidService
    {
        Task<IEnumerable<Bid>> GetAllAsync();
        Task<Bid?> GetByIdAsync(int id);
        Task<Bid> AddBidAsync(int AuctionId, Bid bid);
        Task<Bid> UpdateBidAsync(int AuctionId, Bid bid);  
        Task DeleteBidAsync(int id);
    }
}
