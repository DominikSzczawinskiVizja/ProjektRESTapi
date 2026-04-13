//Miejsce w którym program waliduje dane, koordynuje działanie respozytorium i transformuje dane np mapuje dto na model

using api.Models;
using api.DTOs.AuctionDto;
using api.Repositories.AuctionRepo;

namespace api.Services.AuctionS
{
    public class AuctionService(IAuctionRepository repository) : IAuctionService
    {
        private readonly IAuctionRepository _repository = repository;

        public async Task<IEnumerable<Auction>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Auction?> GetByIdAsync(long id)
        {
            var auction = await _repository.GetByIdAsync(id);
            return auction ?? throw new KeyNotFoundException($"Auction with ID: {id} not found");
        }
        public async Task<Auction> AddAuctionAsync(AuctionCreateDto dto, long ownerId)
        {
            var newAuction = new Auction
            {
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Price = dto.Price,
                EndAt = dto.EndAt,
                CurrentPrice = dto.Price,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow,
            };
            if (newAuction.EndAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException("End date must be in the future");
            }
            return await _repository.AddAuctionAsync(newAuction);
        }
        public async Task<Auction> UpdateAuctionAsync(long id, AuctionUpdateDto dto)
        {
            var auction = await _repository.GetByIdAsync(id);
            if (auction!.Bids.Count != 0)
            {
                throw new InvalidOperationException("Cannot change price when bids already exist");
            }
            auction.Name = dto.Name ?? auction.Name;
            auction.Description = dto.Description ?? auction.Description;
            auction.Category = dto.Category ?? auction.Category;
            auction.Price = dto.Price ?? auction.Price;

            return await _repository.UpdateAuctionAsync(auction);
        }
        public async Task DeleteAuctionAsync(long id)
        {
            var auction = await _repository.GetByIdAsync(id);
            if(auction!.Bids.Count != 0)
            {
                throw new InvalidOperationException("Cannot delete Auction when bids already exist");
            }
            await _repository.DeleteAuctionAsync(id);
        }
    }
}
