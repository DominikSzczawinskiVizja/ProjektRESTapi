using api.Services.BidS;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BidsController : ControllerBase
    {
        private readonly IBidService _service;

        public BidsController(IBidService service)
        {
            _service = service;
        }
    }
}
