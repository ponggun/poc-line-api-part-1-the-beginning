namespace PocLineAPI.Application.Interfaces;

public interface ILineMessagingService
{
    Task<string> LineLoginAsync();
    Task SendMessageAsync(string message, string replyTokenString);
}
