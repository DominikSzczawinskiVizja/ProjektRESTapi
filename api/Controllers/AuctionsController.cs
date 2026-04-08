//przyjmuje requesty http od klienta i odsyla odpowiedzi
using api.DTOs.AuctionDto;
using api.Services.AuctionS;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuctionsController(IAuctionService service) : ControllerBase
    {
        private readonly IAuctionService _service = service;

        //pobieranie aukcji po id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuctionById (long id)
        {
            var auction = await _service.GetByIdAsync(id);
            return Ok(auction);
        }
        //dodawanie aukcji
        [HttpPost]
        public async Task<IActionResult> AddAuction([FromBody] AuctionCreateDto dto)
        {
            var auction = await _service.AddAuctionAsync(dto);
            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, auction);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuction([FromBody] AuctionUpdateDto dto, [FromRoute] long id)
        {
            var auction = await _service.UpdateAuctionAsync(id,dto);
            return Ok(auction);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuction([FromRoute] long id)
        {
            await _service.DeleteAuctionAsync(id);
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAuctions()
        {
            var auction = await _service.GetAllAsync();
            return Ok(auction);
        }

    }
}
