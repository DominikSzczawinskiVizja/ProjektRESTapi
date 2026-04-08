using api.Models;
using api.Services.UserS;
using api.DTOs.UserDto;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(IUserService service) : ControllerBase
    {
        //dodanie serwisow do kontrolera w _service
        private readonly IUserService _service = service;

        //Pobieranie użytkownika po ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            var user = await _service.GetByIdAsync(id);
            return Ok(user);
        }
        //Dodanie Użytkownika
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserCreateDto dto)
        {
            var user = await _service.AddUserAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user); //Zwraca 201 (Created) + adres url, nameof pobiera uzytkownika po id, new przekazuje id dla nameof metoda zwraca wszystko w user
        }
        //Aktualizacja Użytkownika
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto dto, [FromRoute] long id)
        {
            var user = await _service.UpdateUserAsync(id, dto);
            return Ok(user);
        }
        //Usunięcie użytkownika
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] long id)
        {
            await _service.DeleteUserAsync(id);
            return NoContent();
        }
        //lista użytkowników
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }
    }
}
