using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using PocLineAPI.Application.Interfaces;
using Microsoft.Extensions.Options;
using PocLineAPI.Application.Models;
using System.Net.Http.Json;

namespace PocLineAPI.Infrastructure.Services;

public class LineMessagingService : ILineMessagingService
{
    private readonly LineOptions _lineOptions;
    public LineMessagingService(IOptions<LineOptions> lineOptions)
    {
        _lineOptions = lineOptions.Value;
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
        var response = await client.PostAsync("https://api.line.me/v2/oauth/accessToken", content);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonResponse);
        return tokenResponse?.access_token ?? string.Empty;
    }

    public async Task SendMessageAsync(string message, string replyTokenString)
    {
        var accessToken = await LineLoginAsync();
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        await client.PostAsJsonAsync("https://api.line.me/v2/bot/message/reply", new
        {
            messages = new[] {
                new {
                    type = "text",
                    text = message
                }
            },
            replyToken = replyTokenString
        });
        // Logging can be added here if needed
    }
}
