using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.Extensions.Options;
using System.Text.Json;
using PocLineAPI.Application.Interfaces;
using PocLineAPI.Application.Models;

namespace PocLineAPI.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LineWebhookController : ControllerBase
{
    private readonly ILogger<LineWebhookController> _logger;
    private readonly PocLineAPI.Application.Models.LineOptions _lineOptions;
    private readonly ILineMessagingService _LineMessagingService;

    public LineWebhookController(ILogger<LineWebhookController> logger, IOptions<PocLineAPI.Application.Models.LineOptions> lineOptions, ILineMessagingService LineMessagingService)
    {
        _logger = logger;
        _lineOptions = lineOptions.Value;
        _LineMessagingService = LineMessagingService;
    }

    [HttpPost("ReceiveHook")]
    public async Task<IActionResult> ReceiveHook()
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();

        var lineSignature = Request.Headers["X-Line-Signature"].FirstOrDefault();
        _logger.LogInformation("Received LINE webhook: {Body}", body ?? string.Empty);
        _logger.LogInformation("X-Line-Signature: {Signature}", lineSignature ?? string.Empty);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Ensure correct casing for JSON keys
            AllowTrailingCommas = true
        };

        if (string.IsNullOrEmpty(body))
        {
            return BadRequest("Request body is empty.");
        }

        var webhookRequest = JsonSerializer.Deserialize<LineMessagingAPI.WebhookRequest>(body, options);

        if (webhookRequest == null)
        {
            return BadRequest("Invalid JSON format.");
        }

        if (string.IsNullOrEmpty(lineSignature) || !_LineMessagingService.VerifySignature(_lineOptions.ChannelSecret, body, lineSignature))
        {
            _logger.LogError("Invalid LINE signature.");
            return Unauthorized("Invalid LINE signature.");
        }

        string? replyToken = null;
        if (webhookRequest.Events != null)
        {
            foreach (var @event in webhookRequest.Events)
            {
                replyToken = @event.ReplyToken;

                if (@event.Message != null && !string.IsNullOrEmpty(@event.Message.Text) && !string.IsNullOrEmpty(replyToken))
                {
                    _logger.LogInformation("Message: {Body}", @event.Message.Text);
                    await _LineMessagingService.SendMessageAsync(@event.Message.Text, replyToken);
                }
                else
                {
                    _logger.LogInformation("Message: {Body}", string.Empty);
                }

            }
        }

        return Ok(new { Result = $"Done getting web hook" });
    }

    [HttpPost("GenerateSignature")]
    public async Task<IActionResult> GenerateSignature()
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();

        var signature = _LineMessagingService.GenerateSignature(_lineOptions.ChannelSecret, body);
        Console.WriteLine("X-Line-Signature: " + signature);

        return Ok(new { Signature = signature });
    }
}
