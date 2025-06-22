using PocLineAPI.Domain;

namespace PocLineAPI.Application;

public interface IWebhookEventBusinessService
{
    Task<IEnumerable<WebhookEvent>> GetAllAsync();
    Task<WebhookEvent?> GetByIdAsync(Guid id);
    Task<WebhookEvent> AddAsync(WebhookEvent webhookEvent);
    Task<bool> UpdateAsync(WebhookEvent webhookEvent);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

public class WebhookEventBusinessService : IWebhookEventBusinessService
{
    private readonly IWebhookEventRepository _repository;

    public WebhookEventBusinessService(IWebhookEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<WebhookEvent>> GetAllAsync()
        => await _repository.GetAllAsync();

    public async Task<WebhookEvent?> GetByIdAsync(Guid id)
        => await _repository.GetByIdAsync(id);

    public async Task<WebhookEvent> AddAsync(WebhookEvent webhookEvent)
    {
        await _repository.AddAsync(webhookEvent);
        return webhookEvent;
    }

    public async Task<bool> UpdateAsync(WebhookEvent webhookEvent)
    {
        var existing = await _repository.GetByIdAsync(webhookEvent.Id);
        if (existing == null) return false;
        await _repository.UpdateAsync(webhookEvent);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;
        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity != null;
    }
}

