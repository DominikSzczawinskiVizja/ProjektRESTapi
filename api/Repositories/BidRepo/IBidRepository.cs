using api.Models;

namespace api.Repositories.BidRepo
{
    public interface IBidRepository
    {
        Task<IEnumerable<Bid>> GetAllAsync();
        Task<Bid?> GetByIdAsync(long id);
        Task<Bid> AddBidAsync(Bid bid);
        Task<Bid> UpdateBidAsync(Bid bid);
        Task DeleteBidAsync(long id);
    }
}
