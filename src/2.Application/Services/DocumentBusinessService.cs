using PocLineAPI.Application.Interfaces;
using PocLineAPI.Domain.Entities;
using PocLineAPI.Domain.Interfaces;

namespace PocLineAPI.Application.Services
{
    /// <summary>
    /// Implementation of the document service.
    /// </summary>
    public class DocumentBusinessService : IDocumentBusinessService
    {
        private readonly IRepository _repository;
        private readonly IEmbeddingInfraService _EmbeddingInfraService;

        public DocumentBusinessService(IRepository repository, IEmbeddingInfraService EmbeddingInfraService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _EmbeddingInfraService = EmbeddingInfraService ?? throw new ArgumentNullException(nameof(EmbeddingInfraService));
        }

        public async Task<bool> CreateDocumentAsync(Document document)
        {
            // Business logic for creating a document would go here
            return await _repository.AddAsync(document);
        }

        public async Task<bool> DeleteDocumentAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Document> GetDocumentByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Document>> SearchSimilarDocumentsAsync(string query, int limit = 5)
        {
            // Business logic for semantic search would go here
            // This is a placeholder implementation
            throw new NotImplementedException("Semantic search not yet implemented");
        }

        public async Task<bool> UpdateDocumentAsync(Document document)
        {
            return await _repository.UpdateAsync(document);
        }
    }
}