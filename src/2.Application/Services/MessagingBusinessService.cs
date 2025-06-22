using System.Text.Json;
using Microsoft.Extensions.Logging;
using PocLineAPI.Domain;

namespace PocLineAPI.Application;

/// <summary>
/// Interface for messaging business service.
/// </summary>
public interface IMessagingBusinessService
{
    /// <summary>
    /// Handles the incoming LINE webhook request.
    /// </summary>
    /// <param name="body">The raw request body.</param>
    /// <param name="signature">The LINE signature header.</param>
    Task HandleWebhookAsync(string body, string? signature);

    /// <summary>
    /// Generates a signature for the given request body.
    /// </summary>
    /// <param name="body">The raw request body.</param>
    /// <returns>The generated signature.</returns>
    Task<string> GenerateSignatureAsync(string body);
}

/// <summary>
/// Implementation of the messaging business service.
/// </summary>
public class MessagingBusinessService : IMessagingBusinessService
{
    private readonly ILineMessagingInfraService _lineInfraService;
    private readonly ILogger<MessagingBusinessService> _logger;
    private readonly IWebhookEventBusinessService _webhookEventBusinessService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessagingBusinessService"/> class.
    /// </summary>
    /// <param name="lineInfraService">The LINE messaging infrastructure service.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="webhookEventBusinessService">The webhook event business service.</param>
    public MessagingBusinessService(
        ILineMessagingInfraService lineInfraService,
        ILogger<MessagingBusinessService> logger,
        IWebhookEventBusinessService webhookEventBusinessService)
    {
        _lineInfraService = lineInfraService;
        _logger = logger;
        _webhookEventBusinessService = webhookEventBusinessService;
    }

    /// <inheritdoc/>
    public async Task HandleWebhookAsync(string body, string? signature)
    {
        ValidateSignatureOrThrow(body, signature);
        var webhookRequest = DeserializeWebhookRequestOrThrow(body);
        if (webhookRequest.Events == null)
            return;

        var accessToken = await GetAccessTokenOrThrow();
        foreach (var lineEvent in webhookRequest.Events)
        {
            await ProcessLineEventAsync(lineEvent, accessToken);
        }
    }

    /// <inheritdoc/>
    public Task<string> GenerateSignatureAsync(string body)
    {
        var signature = _lineInfraService.GenerateSignature(body);
        return Task.FromResult(signature);
    }

    private void ValidateSignatureOrThrow(string body, string? signature)
    {
        if (string.IsNullOrEmpty(signature) || !_lineInfraService.VerifySignature(body, signature))
        {
            _logger.LogWarning("Invalid LINE signature received.");
            throw new UnauthorizedAccessException("Invalid LINE signature.");
        }
    }

    private LineMessagingAPI.WebhookRequest DeserializeWebhookRequestOrThrow(string body)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };
        var webhookRequest = JsonSerializer.Deserialize<LineMessagingAPI.WebhookRequest>(body, options);
        if (webhookRequest == null)
        {
            _logger.LogError("Invalid JSON format received in webhook body.");
            throw new ArgumentException("Invalid JSON format.");
        }
        return webhookRequest;
    }

    private async Task<string> GetAccessTokenOrThrow()
    {
        var accessToken = await _lineInfraService.LineLoginAsync();
        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogError("Failed to obtain access token from LINE API.");
            throw new InvalidOperationException("Failed to obtain access token.");
        }
        return accessToken;
    }

    private async Task ProcessLineEventAsync(LineMessagingAPI.WebhookRequest.Event lineEvent, string accessToken)
    {
        // Record the webhook event
       var webhookEvent = await RecordWebhookEventFromLineEvent(lineEvent);

        // Extract relevant information from the event using a helper class
        var info = new LineEventInfo(lineEvent);

        // If the event contains a message, reply token, and user ID, send a response
        // Otherwise, log the event without processing
        if (!string.IsNullOrEmpty(info.MessageText) && !string.IsNullOrEmpty(info.ReplyToken) && !string.IsNullOrEmpty(info.UserId))
        {
            try
            {
                await _lineInfraService.LineLoading(accessToken, info.UserId);

                await _lineInfraService.SendMessageAsync(accessToken, info.MessageText, info.ReplyToken);

                await MarkWebhookEventProcessedAsync(webhookEvent);
            }
            catch (Exception ex)
            {
                await HandleLineEventErrorAsync(ex, webhookEvent);
                throw; // Re-throw the exception after logging and updating the event, preserving stack trace
            }
        }
        else
        {
            _logger.LogInformation("Received event without message or reply token: {Event}", JsonSerializer.Serialize(lineEvent));
        }
    }

    private async Task<WebhookEvent> RecordWebhookEventFromLineEvent(LineMessagingAPI.WebhookRequest.Event lineEvent)
    {
        var source = lineEvent.Source;
        var webhookEvent = new WebhookEvent
        {
            EventJson = JsonDocument.Parse(JsonSerializer.Serialize(lineEvent)),
            EventType = lineEvent.Type,
            CreatedAt = DateTimeOffset.UtcNow,
            Processed = false,
            LineWebhookEventId = lineEvent.WebhookEventId,
            SourceType = source?.Type,
            GroupId = source?.GroupId,
            UserId = source?.UserId
        };

        return await _webhookEventBusinessService.AddAsync(webhookEvent);
    }

    private async Task MarkWebhookEventProcessedAsync(WebhookEvent webhookEvent)
    {
        webhookEvent.Processed = true;
        webhookEvent.ProcessedAt = DateTimeOffset.UtcNow;
        
        await _webhookEventBusinessService.UpdateAsync(webhookEvent);
    }

    private async Task HandleLineEventErrorAsync(Exception ex, WebhookEvent webhookEvent)
    {
        _logger.LogError(ex, "Error processing LINE event");
        webhookEvent.ErrorMessage = ex.StackTrace ?? ex.Message;
        // Do not set as processed, just update the event with the error
        await _webhookEventBusinessService.UpdateAsync(webhookEvent);
    }

    private sealed class LineEventInfo
    {
        public string? MessageText { get; }
        public string? ReplyToken { get; }
        public string? UserId { get; }

        public LineEventInfo(LineMessagingAPI.WebhookRequest.Event lineEvent)
        {
            MessageText = lineEvent.Message?.Text;
            ReplyToken = lineEvent.ReplyToken;
            UserId = lineEvent.Source?.UserId;
        }
    }
}