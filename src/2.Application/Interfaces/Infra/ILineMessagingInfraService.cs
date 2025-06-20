namespace PocLineAPI.Application.Interfaces;

public interface ILineMessagingInfraService
{
    Task<string> LineLoginAsync();
    Task SendMessageAsync(string message, string replyTokenString);
    bool VerifySignature(string requestBody, string? signature);
    string GenerateSignature(string body);
}
