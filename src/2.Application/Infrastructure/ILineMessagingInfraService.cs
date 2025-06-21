namespace PocLineAPI.Application;

public interface ILineMessagingInfraService
{
    Task<string> LineLoginAsync();
    Task SendMessageAsync(string accessToken, string message, string replyTokenString);
    bool VerifySignature(string requestBody, string? signature);
    string GenerateSignature(string body);
    Task LineLoading(string accessToken, string userId);
}
