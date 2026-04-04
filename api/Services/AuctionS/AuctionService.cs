using api.Models;
using api.Repositories.AuctionRepo;
using NuGet.Protocol.Core.Types;

namespace api.Services.AuctionS
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _repository;

        public AuctionService(IAuctionRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Auction>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Auction?> GetByIdAsync(long id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<Auction> AddAuctionAsync(Auction auction)
        {
            return await _repository.AddAuctionAsync(auction);
        }
        public async Task<Auction> UpdateAuctionAsync(Auction auction)
        {
            return await _repository.UpdateAuctionAsync(auction);
        }
        public async Task DeleteAuctionAsync(long id)
        {
            await _repository.DeleteAuctionAsync(id);
        }
    }
}
