using api.Models;

namespace api.DTOs.UserDto
{
    public static class UserMappings
    {
        public static UserResponseDto ToResponseDto(this User user) => new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}
