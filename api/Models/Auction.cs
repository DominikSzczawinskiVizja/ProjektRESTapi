namespace api.Models
{
    public class Auction
    {
        public ICollection<Bid> Bids { get; set; } = [];

        public long Id { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string Category { get; set; }

        public required decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public long OwnerId { get; set; }

        public decimal CurrentPrice { get; set; }

    }
}
