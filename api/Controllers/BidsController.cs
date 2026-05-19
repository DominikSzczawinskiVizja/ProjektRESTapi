using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using api.DTOs.BidDto;
using api.Services.BidS;
using Microsoft.AspNetCore.Mvc;
using api.Extensions;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidsController(IBidService service) : ControllerBase
    {
        private readonly IBidService _service = service;

        [Authorize(Roles = "Admin")]
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
            var safeResponse = new
            {
                amount = bid.Amount,
                createdAt = bid.CreatedAt,
                message = "Bid placed succesfully"
            };
            return Ok(safeResponse);
        }
    }
}
