namespace api.Models
{
    public class Bid
    {

        public required long Id { get; set; }

        public required int Price { get; set; }

        public required string BidName { get; set; }

        public required string WhichAuction { get; set; }

    }
}
