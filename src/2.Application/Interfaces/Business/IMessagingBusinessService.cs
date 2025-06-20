using System.Threading.Tasks;

namespace PocLineAPI.Application.Interfaces
{
    public interface IMessagingBusinessService
    {
        Task HandleWebhookAsync(string body, string? signature);
        Task<string> GenerateSignatureAsync(string body);
    }
}
