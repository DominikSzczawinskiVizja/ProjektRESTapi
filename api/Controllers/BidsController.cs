using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using api.DTOs.BidDto;
using api.Services.BidS;
using Microsoft.AspNetCore.Mvc;
using api.Extensions;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BidsController(IBidService service) : ControllerBase
    {
        private readonly IBidService _service = service;

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetBidById (long id)
        {
            var bid = await _service.GetByIdAsync(id);
            return Ok(bid);
        }
        [Authorize]
        [HttpPost("{AuctionId:long}")]
        public async Task<IActionResult> AddBid(long AuctionId, [FromBody] BidCreateDto dto)
        {

            var bid = await _service.AddBidAsync(User.GetUserId(), AuctionId, dto);
            return CreatedAtAction(nameof(GetBidById), new { id = bid.Id }, bid);
        }
    }
}
