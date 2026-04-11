namespace api.DTOs.AuthDto
{
    public class AuthResponseDto
    {
        public required string AccessToken {  get; set; }
        public required long UserId { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
    }
}
