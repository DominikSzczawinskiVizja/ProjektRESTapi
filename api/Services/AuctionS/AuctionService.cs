//Miejsce w którym program waliduje dane, koordynuje działanie respozytorium i transformuje dane np mapuje dto na model

using api.Models;
using api.DTOs.AuctionDto;
using api.Repositories.AuctionRepo;
using api.Repositories.BidRepo;
using api.Repositories.UserRepo;

namespace api.Services.AuctionS
{
    public class AuctionService(IAuctionRepository repository, IBidRepository bid, IUserRepository user) : IAuctionService
    {
        private readonly IAuctionRepository _repository = repository;
        private readonly IBidRepository _bidRepository = bid;
        private readonly IUserRepository _userRepository = user;

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

        public async Task<Response<AuctionWinnerDto>> GetWinnerAsync(long id)
        {
            var response = new Response<AuctionWinnerDto>();
            var HighestBid = await _bidRepository.GetHighestBidAsync(id);

            if (HighestBid == null)
            {
                response.Success = false;
                response.Message = "Couldn't find any bidder for your auction.";
                return response;
            }

            var user = await _userRepository.GetByIdAsync(HighestBid.UserId);
            if (user == null) { response.Success = false; response.Message = "Bidding user was deleted"; return response; }

            var auctionWinner = new AuctionWinnerDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Address = user.Address,
            };
            response.Data = auctionWinner;
            return response;
        }
    }
}
