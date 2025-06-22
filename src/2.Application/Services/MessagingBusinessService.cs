using System.Text.Json;
using Microsoft.Extensions.Logging;
using PocLineAPI.Application.Services;
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
        var messageText = lineEvent.Message?.Text;
        var replyToken = lineEvent.ReplyToken;
        var sourceType = lineEvent.Source?.Type;
        var groupId = lineEvent.Source?.GroupId;
        var userId = lineEvent.Source?.UserId;

        // Map lineEvent to domain WebhookEvent entity
        var webhookEvent = new WebhookEvent
        {
            Id = Guid.NewGuid(),
            EventJson = JsonDocument.Parse(JsonSerializer.Serialize(lineEvent)),
            EventType = lineEvent.Type,
            CreatedAt = DateTimeOffset.UtcNow,
            Processed = false,
            LineWebhookEventId = lineEvent.WebhookEventId,
            SourceType = sourceType,
            GroupId = groupId,
            UserId = userId
        };
        await _webhookEventBusinessService.AddAsync(webhookEvent);

        if (!string.IsNullOrEmpty(messageText) && !string.IsNullOrEmpty(replyToken) && userId != null)
        {
            await _lineInfraService.LineLoading(accessToken, userId);
            _logger.LogInformation(
                "Received message: {Message} from user: {UserId} with reply token: {ReplyToken}",
                (object)(messageText ?? string.Empty),
                (object)(userId ?? string.Empty),
                (object)(replyToken ?? string.Empty)
            );
            await _lineInfraService.SendMessageAsync(accessToken, messageText, replyToken);
        }
        else
        {
            _logger.LogInformation("Received event without message or reply token: {Event}", (object)JsonSerializer.Serialize(lineEvent));
        }
    }
}