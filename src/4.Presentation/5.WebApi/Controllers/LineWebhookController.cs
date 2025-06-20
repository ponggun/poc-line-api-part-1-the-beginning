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

    public LineWebhookController(ILogger<LineWebhookController> logger, IMessagingBusinessService messagingBusinessService)
    {
        _logger = logger;
        _messagingBusinessService = messagingBusinessService;
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
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Invalid LINE signature.");
            return Unauthorized("Invalid LINE signature.");
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid JSON format.");
            return BadRequest("Invalid JSON format.");
        }

        return Ok(new { Result = $"Done getting web hook" });
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
}
