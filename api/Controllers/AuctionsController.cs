//przyjmuje requesty http od klienta i odsyla odpowiedzi
using api.DTOs.AuctionDto;
using api.Services.AuctionS;
using Microsoft.AspNetCore.Mvc;
using api.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController(IAuctionService service) : ControllerBase
    {
        private readonly IAuctionService _service = service;

        //pobieranie aukcji po id
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetAuctionById (long id)
        {
            var auction = await _service.GetByIdAsync(id);
            return Ok(auction);
        }

        //dodawanie aukcji
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAuction([FromBody] AuctionCreateDto dto)
        {
            var ownerId = User.GetUserId();
            var auction = await _service.AddAuctionAsync(dto, ownerId);
            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, auction);
        }

        [Authorize(Policy = "UserRoleStatus")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateAuction([FromBody] AuctionUpdateDto dto, [FromRoute] long id)
        {
            var auction = await _service.UpdateAuctionAsync(id,dto);
            return Ok(auction);
        }

        [Authorize(Policy = "UserRoleStatus")]
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteAuction([FromRoute] long id)
        {
            await _service.DeleteAuctionAsync(id);
            return NoContent();
        }

        [Authorize(Policy = "UserRoleStatus")]
        [HttpGet("{id:long}/winner")]
        public async Task<IActionResult> GetAuctionWinner([FromRoute] long id)
        {
            var result = await _service.GetWinnerAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuctions()
        {
            var auction = await _service.GetAllAsync();
            return Ok(auction);
        }
    }
}
