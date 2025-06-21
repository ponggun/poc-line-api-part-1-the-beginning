using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PocLineAPI.Application;

public interface IMessagingBusinessService
{
    Task HandleWebhookAsync(string body, string? signature);
    Task<string> GenerateSignatureAsync(string body);
}

public class MessagingBusinessService : IMessagingBusinessService
{
    private readonly ILineMessagingInfraService _infraService;
    private readonly ILogger<MessagingBusinessService> _logger;

    public MessagingBusinessService(ILineMessagingInfraService infraService, ILogger<MessagingBusinessService> logger)
    {
        _infraService = infraService;
        _logger = logger;
    }

    public async Task HandleWebhookAsync(string body, string? signature)
    {
        if (string.IsNullOrEmpty(signature) || !_infraService.VerifySignature(body, signature))
        {
            _logger.LogWarning("Invalid LINE signature received.");
            throw new UnauthorizedAccessException("Invalid LINE signature.");
        }

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

        if (webhookRequest.Events != null)
        {
            var accessToken = await _infraService.LineLoginAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogError("Failed to obtain access token from LINE API.");
                throw new InvalidOperationException("Failed to obtain access token.");
            }

            foreach (var lineEvent in webhookRequest.Events)
            {
                var replyToken = lineEvent.ReplyToken;


                if (lineEvent.Message != null && !string.IsNullOrEmpty(lineEvent.Message.Text) && !string.IsNullOrEmpty(replyToken) && lineEvent.Source?.UserId != null)
                {
                    await _infraService.LineLoading(accessToken, lineEvent.Source.UserId);

                    _logger.LogInformation("Received message: {Message} from user: {UserId} with reply token: {ReplyToken}", lineEvent.Message.Text, lineEvent.Source?.UserId, replyToken);
                    await _infraService.SendMessageAsync(accessToken, lineEvent.Message.Text, replyToken);
                }
                else
                {
                    _logger.LogInformation("Received event without message or reply token: {Event}", JsonSerializer.Serialize(lineEvent));
                }
            }
        }
    }

    public Task<string> GenerateSignatureAsync(string body)
    {
        var signature = _infraService.GenerateSignature(body);
        return Task.FromResult(signature);
    }
}