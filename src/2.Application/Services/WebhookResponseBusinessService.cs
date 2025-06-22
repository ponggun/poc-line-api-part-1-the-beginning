using PocLineAPI.Domain;

namespace PocLineAPI.Application;

public interface IWebhookResponseBusinessService
{
    Task<IEnumerable<WebhookResponse>> GetAllAsync();
    Task<WebhookResponse?> GetByIdAsync(Guid id);
    Task AddAsync(WebhookResponse webhookResponse);
    Task UpdateAsync(WebhookResponse webhookResponse);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

public class WebhookResponseBusinessService : IWebhookResponseBusinessService
{
    private readonly IWebhookResponseRepository _repository;

    public WebhookResponseBusinessService(IWebhookResponseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<WebhookResponse>> GetAllAsync()
        => await _repository.GetAllAsync();

    public async Task<WebhookResponse?> GetByIdAsync(Guid id)
        => await _repository.GetByIdAsync(id);

    public async Task AddAsync(WebhookResponse webhookResponse)
        => await _repository.AddAsync(webhookResponse);

    public async Task UpdateAsync(WebhookResponse webhookResponse)
        => await _repository.UpdateAsync(webhookResponse);

    public async Task DeleteAsync(Guid id)
        => await _repository.DeleteAsync(id);

    public async Task<bool> ExistsAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity != null;
    }
}