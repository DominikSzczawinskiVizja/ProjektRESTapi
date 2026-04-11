//Miejsce w którym program waliduje dane
using System.Security.Claims;
using api.DTOs.BidDto;
using api.Models;
using api.Repositories.BidRepo;
using api.Repositories.AuctionRepo;

namespace api.Services.BidS
{
        public class BidService(IBidRepository repository, IAuctionRepository auction, IHttpContextAccessor httpContextAccessor) : IBidService
        {
        private readonly IBidRepository _repository = repository;
        private readonly IAuctionRepository _auction = auction;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<IEnumerable<Bid>> GetAllAsync() //get all bids
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Bid?> GetByIdAsync(long id) //getById
        {
            return await _repository.GetByIdAsync(id);

        }
        public async Task<Bid> AddBidAsync(long UserId, long AuctionId, BidCreateDto dto) //add
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier); //pobieranie id z tokena

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("You must be logged in to place a bid.");
            }
            long currentUserId = long.Parse(userIdClaim.Value);

            var Auction = await _auction.GetByIdAsync(AuctionId);

            if (Auction!.EndAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException($"The Auction with ID: {AuctionId} has already ended.");
            }

            if (dto.Amount <= Auction.CurrentPrice)
            {
                throw new InvalidOperationException($"The bid amount must be higher than the current price of the auction. Current price: {Auction.CurrentPrice}");
            }

            var newBid = new Bid
            {
                Amount = dto.Amount,
                AuctionId = AuctionId,
                UserId = UserId,
                CreatedAt = DateTime.UtcNow,
            };
            Auction.CurrentPrice = newBid.Amount;
            await _auction.UpdateAuctionAsync(Auction);
            return await _repository.AddBidAsync(newBid);
        }
        public async Task<Bid> UpdateBidAsync(long AuctionId, Bid bid) //update
        {
            var Auction = await _auction.GetByIdAsync(AuctionId);
            if (Auction!.EndAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException($"The Auction with ID: {AuctionId} has already ended.");
            }
            if (bid.Amount <= Auction.CurrentPrice)
            {
                throw new InvalidOperationException($"The bid amount must be higher than the current price of the auction. Current price: {Auction.CurrentPrice}");
            }
            Auction.CurrentPrice = bid.Amount;
            await _auction.UpdateAuctionAsync(Auction);
            return await _repository.UpdateBidAsync(bid);
        }
        public async Task DeleteBidAsync(long id) //delete
            {
                await _repository.DeleteBidAsync(id);
            }
        }
    }

