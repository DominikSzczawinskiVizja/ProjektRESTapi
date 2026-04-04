using api.Models;
using api.Repositories.BidRepo;
using api.Repositories.AuctionRepo;
using api.Services.BidS;

namespace api.Services.BidS
{
        public class BidService : IBidService
        {
        private readonly IBidRepository _repository;
        private readonly IAuctionRepository _auction;

        public BidService(IBidRepository repository, IAuctionRepository auction)
            {
                _repository = repository;
                _auction = auction;
        }
        public async Task<IEnumerable<Bid>> GetAllAsync() //get all bids
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Bid?> GetByIdAsync(int id) //getById
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<Bid> AddBidAsync(int AuctionId, Bid bid) //add
        {
            var Auction = await _auction.GetByIdAsync(AuctionId);
            if (Auction == null)
            {
                throw new KeyNotFoundException($"There is no Auction with ID: {AuctionId}");
            }
            if (Auction.EndAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException($"The Auction with ID: {AuctionId} has already ended.");
            }
            if (bid.Amount <= Auction.CurrentPrice)
            {
                throw new InvalidOperationException($"The bid amount must be higher than the current price of the auction. Current price: {Auction.CurrentPrice}");
            }
            Auction.CurrentPrice = bid.Amount;
            await _auction.UpdateAuctionAsync(Auction);
            return await _repository.AddBidAsync(bid);
        }
        public async Task<Bid> UpdateBidAsync(int AuctionId, Bid bid) //update
        {
            var Auction = await _auction.GetByIdAsync(AuctionId);
            if (Auction == null)
            {
                throw new KeyNotFoundException($"There is no Auction with ID: {AuctionId}");
            }
            if (Auction.EndAt < DateTime.UtcNow)
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
        public async Task DeleteBidAsync(int id) //delete
            {
                await _repository.DeleteBidAsync(id);
            }
        }
    }

