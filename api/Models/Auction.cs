namespace api.Models
{
    public class Auction
    {
        public required long Id { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string Category { get; set; }

        public required int Price { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime EndAt { get; set; }

        public required string OwnerName { get; set; }

    }
}
