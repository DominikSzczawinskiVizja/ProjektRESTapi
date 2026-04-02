namespace api.Models
{
    public class User
    {
        public required long Id { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string Passwrdh { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
