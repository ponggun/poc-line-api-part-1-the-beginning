using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PocLineAPI.Application;

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
                foreach (var @event in webhookRequest.Events)
                {
                    var replyToken = @event.ReplyToken;
                    if (@event.Message != null && !string.IsNullOrEmpty(@event.Message.Text) && !string.IsNullOrEmpty(replyToken))
                    {
                        _logger.LogInformation("Received message: {Message} from user: {UserId} with reply token: {ReplyToken}", @event.Message.Text, @event.Source?.UserId, replyToken);
                        await _infraService.SendMessageAsync(@event.Message.Text, replyToken);
                    }
                    else
                    {
                        _logger.LogInformation("Received event without message or reply token: {Event}", JsonSerializer.Serialize(@event));
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