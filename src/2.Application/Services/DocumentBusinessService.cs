using PocLineAPI.Domain;
using PocLineAPI.Application;
using Microsoft.Extensions.Logging;

namespace PocLineAPI.Application;

public interface IDocumentBusinessService 
{
    Task<Document?> GetByIdAsync(Guid id);
    Task<IEnumerable<Document>> GetAllAsync();
    Task AddAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(Guid id);
}

public class DocumentBusinessService : IDocumentBusinessService
{
    private readonly IDocumentRepository _repository;
    private readonly ILogger<DocumentBusinessService> _logger;

    public DocumentBusinessService(IDocumentRepository repository, ILogger<DocumentBusinessService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Document?> GetByIdAsync(Guid id)
    {
        var doc = await _repository.GetByIdAsync(id);
        if (doc == null)
        {
            _logger.LogWarning("Document with id {Id} not found.", id);
        }
        return doc;
    }

    public async Task<IEnumerable<Document>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task AddAsync(Document document)
    {
        await _repository.AddAsync(document);
        _logger.LogInformation("Added document with id {Id}.", document.Id);
    }

    public async Task UpdateAsync(Document document)
    {
        await _repository.UpdateAsync(document);
        _logger.LogInformation("Updated document with id {Id}.", document.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        _logger.LogInformation("Deleted document with id {Id}.", id);
    }
}
