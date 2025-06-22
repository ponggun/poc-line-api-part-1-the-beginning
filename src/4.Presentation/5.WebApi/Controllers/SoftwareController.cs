using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application;

namespace PocLineAPI.Presentation.WebApi;

[ApiController]
[Route("api/[controller]")]
public class SoftwareController : ControllerBase
{
    private readonly ISoftwareBusinessService _SoftwareBusinessService;
    private readonly IErrorLogBusinessService _errorLogBusinessService;

    public SoftwareController(ISoftwareBusinessService SoftwareBusinessService, IErrorLogBusinessService ErrorLogBusinessService)
    {
        _SoftwareBusinessService = SoftwareBusinessService;
        _errorLogBusinessService = ErrorLogBusinessService;
    }

    [HttpGet("version")]
    public IActionResult GetVersion()
    {
        try
        {
            return Ok(new { version = _SoftwareBusinessService.GetVersion() });
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogBusinessService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }
}

