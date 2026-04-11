using api.DTOs.BidDto;
using api.Models;

namespace api.Services.BidS
{
    public interface IBidService
    {
        Task<IEnumerable<Bid>> GetAllAsync();
        Task<Bid?> GetByIdAsync(long id);
        Task<Bid> AddBidAsync(long UserId, long AuctionId, BidCreateDto dto);
        Task<Bid> UpdateBidAsync(long AuctionId, Bid bid);  
        Task DeleteBidAsync(long id);
    }
}
