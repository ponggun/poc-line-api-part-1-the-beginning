using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PocLineAPI.Presentation.WebApi.Options;

namespace PocLineAPI.Presentation.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SoftwareController : ControllerBase
    {
        private readonly SoftwareOptions _softwareOptions;

        public SoftwareController(IOptions<SoftwareOptions> softwareOptions)
        {
            _softwareOptions = softwareOptions.Value;
        }

        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok(new { version = _softwareOptions.Version });
        }
    }
}
