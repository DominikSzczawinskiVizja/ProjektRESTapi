namespace api.DTOs.BidDto
{
    public class BidResponseDto
    {
        public long Id { get; set; }

        public decimal Amount { get; set; }

        public string UserFirstName { get; set; } = null!;

        public string? UserMiddleName { get; set; }

        public string UserLastName { get; set; } = null!;

        public string AuctionName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public long AuctionId { get; set; }

        public long UserId { get; set; }    
    }
}
