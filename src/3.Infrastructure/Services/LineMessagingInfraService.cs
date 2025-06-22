using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using PocLineAPI.Application;
using System.Net.Http.Json;
using System.Security.Cryptography;

namespace PocLineAPI.Infrastructure;

public class LineMessagingInfraService : ILineMessagingInfraService
{
    private readonly LineOptions _lineOptions;
    private readonly ILogger<LineMessagingInfraService> _logger;
    public LineMessagingInfraService(IOptions<LineOptions> lineOptions, ILogger<LineMessagingInfraService> logger)
    {
        _lineOptions = lineOptions.Value;
        _logger = logger;
    }

    public async Task<string> LineLoginAsync()
    {
        var client = new HttpClient();
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", _lineOptions.ChannelId),
            new KeyValuePair<string, string>("client_secret", _lineOptions.ChannelSecret),
        });
        
        // Issue stateless channel access token
        var response = await client.PostAsync($"{_lineOptions.APIBaseUrl}/oauth2/v3/token", content);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<LineMessagingAPI.TokenResponse>(jsonResponse);
        return tokenResponse?.access_token ?? string.Empty;
    }

    public async Task SendMessageAsync(string accessToken, string message, string replyTokenString)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        _logger.LogInformation("Sending message: {Message} with reply token: {ReplyToken}", message, replyTokenString);

        await client.PostAsJsonAsync($"{_lineOptions.APIBaseUrl}/v2/bot/message/reply", new
        {
            messages = new[] {
                new {
                    type = "text",
                    text = message
                }
            },
            replyToken = replyTokenString
        });
    }

    public bool VerifySignature(string requestBody, string? signature)
    {
        if (string.IsNullOrEmpty(signature)) return false;
        var key = Encoding.UTF8.GetBytes(_lineOptions.ChannelSecret);
        var bodyBytes = Encoding.UTF8.GetBytes(requestBody);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(bodyBytes);
        var computedSignature = Convert.ToBase64String(hash);

        _logger.LogInformation("Computed Signature: {ComputedSignature}", computedSignature ?? string.Empty);

        return computedSignature == signature;
    }

    public string GenerateSignature(string body)
    {
        var bogyGenerated = body ?? string.Empty;
        if (bogyGenerated == null || string.IsNullOrEmpty(bogyGenerated))
        {
            var errorMessage = "Request body is empty. Cannot generate signature.";

            _logger.LogError(errorMessage);
            throw new ArgumentException(errorMessage);
        }

        _logger.LogInformation("Generating signature for body: {Body}", bogyGenerated);

        var key = Encoding.UTF8.GetBytes(_lineOptions.ChannelSecret);
        var bodyBytes = Encoding.UTF8.GetBytes(bogyGenerated);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(bodyBytes);

        _logger.LogInformation("Generated Signature: {Signature}", Convert.ToBase64String(hash) ?? string.Empty);

        return Convert.ToBase64String(hash);
    }

    public async Task LineLoading(string accessToken, string userId)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        _logger.LogInformation("Starting loading for user: {UserId}", userId);
        
        await client.PostAsJsonAsync($"{_lineOptions.APIBaseUrl}/v2/bot/chat/loading/start", new
        {
            chatId = userId,
            loadingSeconds = 60,
        });
    }
}
