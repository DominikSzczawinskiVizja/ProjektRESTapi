namespace api.DTOs.UserDto
{
    public class UserResponseDto
    {
        public long Id { get; init; }
        public required string FirstName { get; init; }
        public string? MiddleName { get; init; }
        public required string LastName { get; init; }
        public required string Email { get; init; }
        public required string Role { get; init; }
        public DateTime CreatedAt { get; init; }

    }
}
