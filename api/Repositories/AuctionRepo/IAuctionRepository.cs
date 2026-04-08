//Interfejs respozytorium który pokazuje dla repo jakie metody mają sie tam znależć ułatwia testowanie i ułatwia zmianę implementacji
using api.Models;

namespace api.Repositories.AuctionRepo
{
    public interface IAuctionRepository
    {
        Task<IEnumerable<Auction>> GetAllAsync();

        Task<Auction?> GetByIdAsync(long id);

        Task<Auction> AddAuctionAsync(Auction auction);

        Task<Auction> UpdateAuctionAsync(Auction auction);

        Task DeleteAuctionAsync(long id);
    }
}
