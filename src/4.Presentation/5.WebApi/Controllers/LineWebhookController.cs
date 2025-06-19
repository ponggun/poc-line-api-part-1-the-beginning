using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using System.Text.Json;
using PocLineAPI.Presentation.WebApi.Options;
using PocLineAPI.Presentation.WebApi.Models;

namespace PocLineAPI.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LineWebhookController : ControllerBase
{
    private readonly ILogger<LineWebhookController> _logger;
    private readonly LineOptions _lineOptions;


    public LineWebhookController(ILogger<LineWebhookController> logger, IOptions<LineOptions> lineOptions)
    {
        _logger = logger;
        _lineOptions = lineOptions.Value;
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

        if (string.IsNullOrEmpty(lineSignature) || !VerifySignature(_lineOptions.ChannelSecret, body, lineSignature))
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

                if (@event.Message != null)
                {
                    _logger.LogInformation("Message: {Body}", @event.Message.Text ?? string.Empty);
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

        var signature = GenerateSignature(_lineOptions.ChannelSecret, body);
        Console.WriteLine("X-Line-Signature: " + signature);

        return Ok(new { Signature = signature });
    }

    private bool VerifySignature(string channelSecret, string requestBody, string? signature)
    {
        if (string.IsNullOrEmpty(signature)) return false;
        var key = Encoding.UTF8.GetBytes(channelSecret);
        var bodyBytes = Encoding.UTF8.GetBytes(requestBody);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(bodyBytes);
        var computedSignature = Convert.ToBase64String(hash);

        _logger.LogInformation("Computed Signature: {ComputedSignature}", computedSignature ?? string.Empty);

        return computedSignature == signature;
    }

    private static string GenerateSignature(string secret, string body)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var bodyBytes = Encoding.UTF8.GetBytes(body);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(bodyBytes);
        return Convert.ToBase64String(hash);
    }

}
