//Interfejs respozytorium który pokazuje dla repo jakie metody mają sie tam znależć ułatwia testowanie i implementacje
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
