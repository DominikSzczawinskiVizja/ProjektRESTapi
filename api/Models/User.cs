//Struktura bazy danych każda klasa = tabelka w SQL
namespace api.Models
{
    public class User
    {
        public ICollection<Auction> Auctions { get; set; } = [];
        public long Id { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Email { get; set; }
        public string Role { get; set; } = "User";
    }
}
