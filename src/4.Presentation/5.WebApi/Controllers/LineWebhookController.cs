using Microsoft.AspNetCore.Mvc;
using System.Text;
using PocLineAPI.Application;

namespace PocLineAPI.Presentation.WebApi;

[ApiController]
[Route("api/[controller]")]
public class LineWebhookController : ControllerBase
{
    private readonly ILogger<LineWebhookController> _logger;
    private readonly IMessagingBusinessService _messagingBusinessService;
    private readonly IErrorLogBusinessService _errorLogBusinessService;

    public LineWebhookController(ILogger<LineWebhookController> logger, IMessagingBusinessService messagingBusinessService, IErrorLogBusinessService ErrorLogBusinessService)
    {
        _logger = logger;
        _messagingBusinessService = messagingBusinessService;
        _errorLogBusinessService = ErrorLogBusinessService;
    }

    [HttpPost("GenerateSignature")]
    public async Task<IActionResult> GenerateSignature()
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();
        var signature = await _messagingBusinessService.GenerateSignatureAsync(body);

        _logger.LogInformation("X-Line-Signature: " + signature);

        return Ok(new { Signature = signature });
    }

    [HttpPost("ReceiveHook")]
    public async Task<IActionResult> ReceiveHook()
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();
        var lineSignature = Request.Headers["X-Line-Signature"].FirstOrDefault();

        _logger.LogInformation("Received LINE webhook: {Body}", body ?? string.Empty);
        _logger.LogInformation("X-Line-Signature: {Signature}", lineSignature ?? string.Empty);

        if (string.IsNullOrEmpty(body))
        {
            return BadRequest("Request body is empty.");
        }

        try
        {
            await _messagingBusinessService.HandleWebhookAsync(body, lineSignature);
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogBusinessService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }

        return Ok(new { Result = $"Done getting web hook" });
    }
}
