using api.Models;

namespace api.Services.BidS
{
    public interface IBidService
    {
        Task<IEnumerable<Bid>> GetAllAsync();
        Task<Bid?> GetByIdAsync(long id);
        Task<Bid> AddBidAsync(int AuctionId, Bid bid);
        Task<Bid> UpdateBidAsync(int AuctionId, Bid bid);  
        Task DeleteBidAsync(long id);
    }
}
