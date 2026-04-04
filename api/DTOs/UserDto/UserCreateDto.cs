using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs.UserDto
{
    public class UserCreateDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$", ErrorMessage = "First name can only contain letters")]
        public required string FirstName { get; set; }
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$", ErrorMessage = "Middle name can only contain letters")]
        public string? MiddleName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$", ErrorMessage = "Last name can only contain letters")]
        public required string LastName { get; set; }
        [Required]
        [PasswordPropertyText]
        public required string Password { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

    }
}
