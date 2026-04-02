using api.Models;

namespace api.Repositories.AuctionRepo
{
    public interface IAuctionRepository
    {
        Task<IEnumerable<Auction>> GetAllAsync();

        Task<Auction?> GetByIdAsync(int id);

        Task AddAuctionAsync(Auction auction);

        Task UpdateAuctionAsync(Auction auction);

        Task DeleteAuctionAsync(int id);
    }
}
