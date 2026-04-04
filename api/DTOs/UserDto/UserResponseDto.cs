#nullable disable
namespace api.DTOs.UserDto
{
    public class UserResponseDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
#nullable enable
        public string? MiddleName { get; set; }
#nullable disable
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
