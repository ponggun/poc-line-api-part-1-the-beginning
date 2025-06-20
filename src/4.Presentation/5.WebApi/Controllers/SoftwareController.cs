using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application.Interfaces;
using PocLineAPI.Application.Models;

namespace PocLineAPI.Presentation.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SoftwareController : ControllerBase
    {
        private readonly ISoftwareBusinessService _SoftwareBusinessService;

        public SoftwareController(ISoftwareBusinessService SoftwareBusinessService)
        {
            _SoftwareBusinessService = SoftwareBusinessService;
        }

        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok(new { version = _SoftwareBusinessService.GetVersion() });
        }
    }
}
