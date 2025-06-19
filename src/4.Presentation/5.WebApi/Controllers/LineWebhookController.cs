using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using System.Text.Json;
using PocLineAPI.Presentation.WebApi.Options;
using PocLineAPI.Presentation.WebApi.Models;
using PocLineAPI.Application.Interfaces;

namespace PocLineAPI.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LineWebhookController : ControllerBase
{
    private readonly ILogger<LineWebhookController> _logger;
    private readonly LineOptions _lineOptions;
    private readonly ISignatureService _signatureService;
    private readonly ILineMessagingService _lineMessagingService;

    public LineWebhookController(ILogger<LineWebhookController> logger, IOptions<LineOptions> lineOptions, ISignatureService signatureService, ILineMessagingService lineMessagingService)
    {
        _logger = logger;
        _lineOptions = lineOptions.Value;
        _signatureService = signatureService;
        _lineMessagingService = lineMessagingService;
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

        if (string.IsNullOrEmpty(lineSignature) || !_signatureService.VerifySignature(_lineOptions.ChannelSecret, body, lineSignature))
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
                    await _lineMessagingService.SendMessageAsync(@event.Message.Text, replyToken);
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

        var signature = _signatureService.GenerateSignature(_lineOptions.ChannelSecret, body);
        Console.WriteLine("X-Line-Signature: " + signature);

        return Ok(new { Signature = signature });
    }
}
