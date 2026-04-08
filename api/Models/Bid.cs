//Struktura bazy danych każda klasa = tabelka w SQL
namespace api.Models
{
    public class Bid
    {

        public long Id { get; set; }

        public required decimal Amount { get; set; }

        public long AuctionId { get; set; }

        public long UserId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
