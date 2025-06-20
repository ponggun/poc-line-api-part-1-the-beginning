namespace PocLineAPI.Application;

public interface IMessagingBusinessService
{
    Task HandleWebhookAsync(string body, string? signature);
    Task<string> GenerateSignatureAsync(string body);
}
