using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application;

namespace PocLineAPI.Presentation.WebApi;

[ApiController]
[Route("api/[controller]")]
public class SoftwareController : ControllerBase
{
    private readonly ISoftwareBusinessService _SoftwareBusinessService;
    private readonly IErrorLogService _errorLogService;

    public SoftwareController(ISoftwareBusinessService SoftwareBusinessService, IErrorLogService errorLogService)
    {
        _SoftwareBusinessService = SoftwareBusinessService;
        _errorLogService = errorLogService;
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
            var errorCode = _errorLogService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }
}

