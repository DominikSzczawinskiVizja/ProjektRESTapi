using api.Services.AuctionS;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _service;

        public AuctionsController(IAuctionService service)
        {
            _service = service;
        }
    }
}
