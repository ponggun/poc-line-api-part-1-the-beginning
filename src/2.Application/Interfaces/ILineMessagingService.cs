namespace PocLineAPI.Application.Interfaces;

public interface ILineMessagingService
{
    Task<string> LineLoginAsync();
    Task SendMessageAsync(string message, string replyTokenString);
    bool VerifySignature(string channelSecret, string requestBody, string? signature);
    string GenerateSignature(string secret, string body);
}
