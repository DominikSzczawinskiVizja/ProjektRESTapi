using api.Models;

namespace api.Services.AuctionS
{
    public interface IAuctionService
    {
        Task<IEnumerable<Auction>> GetAllAsync();
        Task<Auction?> GetByIdAsync(int id);
        Task<Auction> AddAuctionAsync(Auction auction);
        Task<Auction> UpdateAuctionAsync(Auction auction);
        Task DeleteAuctionAsync(int id);

    }
}
