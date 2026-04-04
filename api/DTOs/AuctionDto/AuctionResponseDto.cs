namespace api.DTOs.AuctionDto
{
    public class AuctionResponseDto
    {
        public long Id { get; set; }

        public long OnwerId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Category { get; set; } = null!;

        public decimal Price { get; set; }

        public decimal PriceNow { get; set; }

        public DateTime EndAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
