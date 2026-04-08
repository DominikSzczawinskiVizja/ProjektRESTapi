namespace api.DTOs.AuctionDto
{
    public class AuctionUpdateDto
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }

        public decimal? Price { get; set; }
    }
}
