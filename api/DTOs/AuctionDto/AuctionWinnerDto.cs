namespace api.DTOs.AuctionDto
{
    public class AuctionWinnerDto
    {
        public string FirstName { get; init; } = "";
        public string? MiddleName { get; init; }
        public string LastName { get; init; } = "";
        public string Address { get; init; } = "";
        public decimal Amount { get; init; }
    }
}
