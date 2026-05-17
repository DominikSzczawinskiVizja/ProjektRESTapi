using api.Services.UserS;
using api.DTOs.UserDto;
using api.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService service) : ControllerBase
    {
        //dodanie serwisow do kontrolera w _service
        private readonly IUserService _service = service;


        //Pobieranie użytkownika po ID
        [Authorize]
        [HttpGet("{id:long}")]

        public async Task<ActionResult<UserResponseDto>> GetUserById([FromRoute]long id)
        {
            var user = await _service.GetByIdAsync(id);
            return Ok(user.ToResponseDto());
        }


        //Dodanie Użytkownika
        [Authorize]
        [HttpPost]

        public async Task<ActionResult<UserResponseDto>> AddUser([FromBody] UserCreateDto dto)
        {
            var user = await _service.AddUserAsync(dto);

            //Zwraca 201 (Created) + adres url, nameof pobiera uzytkownika po id, new przekazuje id dla nameof metoda zwraca wszystko w user
            return CreatedAtAction( 
                nameof(GetUserById),
                new { id = user.Id },
                user.ToResponseDto()
            ); 
        }


        //Aktualizacja Użytkownika
        [Authorize]
        [HttpPut("{id:long}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser([FromRoute] long id, [FromBody] UserUpdateDto dto)
        {
            var currentUserId = User.GetUserId();
            var user = await _service.UpdateUserAsync(id, currentUserId, dto);
            return Ok(user.ToResponseDto());
        }


        //Usunięcie użytkownika
        [Authorize]
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteUser([FromRoute] long id)
        {
            var currentUserId = User.GetUserId();
            await _service.DeleteUserAsync(id, currentUserId);
            return NoContent();
        }


        //lista użytkowników
        [Authorize(Roles = "Admin")]
        [HttpGet]

        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
        {
            var users = await _service.GetAllAsync();
            return Ok(users.Select(u => u.ToResponseDto()));
        }
    }
}
