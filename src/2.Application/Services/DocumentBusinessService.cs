using Microsoft.Extensions.Logging;
using PocLineAPI.Domain;

namespace PocLineAPI.Application;

/// <summary>
/// Implementation of the document service.
/// </summary>
public class DocumentBusinessService : IDocumentBusinessService
{
    private readonly IRepository _repository;
    private readonly IEmbeddingInfraService _EmbeddingInfraService;
    private readonly ILogger<DocumentBusinessService> _logger;

    public DocumentBusinessService(IRepository repository, IEmbeddingInfraService EmbeddingInfraService, ILogger<DocumentBusinessService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _EmbeddingInfraService = EmbeddingInfraService ?? throw new ArgumentNullException(nameof(EmbeddingInfraService));
        _logger = logger;
    }

    public async Task<bool> CreateDocumentAsync(Document document)
    {
        _logger.LogInformation("Creating document with ID: {DocumentId}", document.Id);
        // Business logic for creating a document would go here
        var result = await _repository.AddAsync(document);
        _logger.LogInformation("Document created: {DocumentId}, Success: {Result}", document.Id, result);
        return result;
    }

    public async Task<bool> DeleteDocumentAsync(string id)
    {
        _logger.LogInformation("Deleting document with ID: {DocumentId}", id);
        var result = await _repository.DeleteAsync(id);
        _logger.LogInformation("Document deleted: {DocumentId}, Success: {Result}", id, result);
        return result;
    }

    public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
    {
        _logger.LogInformation("Retrieving all documents.");
        var docs = await _repository.GetAllAsync();
        _logger.LogInformation("Retrieved {Count} documents.", docs?.Count() ?? 0);
        return docs ?? Enumerable.Empty<Document>();
    }

    public async Task<Document> GetDocumentByIdAsync(string id)
    {
        _logger.LogInformation("Retrieving document by ID: {DocumentId}", id);
        var doc = await _repository.GetByIdAsync(id);
        _logger.LogInformation("Document retrieved: {DocumentId}", id);
        return doc;
    }

    public Task<IEnumerable<Document>> SearchSimilarDocumentsAsync(string query, int limit = 5)
    {
        _logger.LogInformation("Searching for similar documents. Query: {Query}, Limit: {Limit}", query, limit);
        // Business logic for semantic search would go here
        // This is a placeholder implementation
        throw new NotImplementedException("Semantic search not yet implemented");
    }

    public async Task<bool> UpdateDocumentAsync(Document document)
    {
        _logger.LogInformation("Updating document with ID: {DocumentId}", document.Id);
        var result = await _repository.UpdateAsync(document);
        _logger.LogInformation("Document updated: {DocumentId}, Success: {Result}", document.Id, result);
        return result;
    }
}
