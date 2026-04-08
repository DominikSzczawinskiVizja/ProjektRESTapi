using api.DTOs.AuctionDto;
using api.Models;

namespace api.Services.AuctionS
{
    public interface IAuctionService
    {
        Task<IEnumerable<Auction>> GetAllAsync();
        Task<Auction?> GetByIdAsync(long id);
        Task<Auction> AddAuctionAsync(AuctionCreateDto dto);
        Task<Auction> UpdateAuctionAsync(long id, AuctionUpdateDto dto);
        Task DeleteAuctionAsync(long id);
    }
}
