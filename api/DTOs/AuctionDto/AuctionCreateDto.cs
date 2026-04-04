namespace api.DTOs.AuctionDto
{
    public class AuctionCreateDto
    {

        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string Category { get; set; }

        public required decimal Price { get; set; }

        public required DateTime EndAt { get; set; }

    }
}
