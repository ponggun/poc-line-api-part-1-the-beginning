using PocLineAPI.Domain.Entities;

namespace PocLineAPI.Application.Interfaces
{
    /// <summary>
    /// Service for managing documents.
    /// </summary>
    public interface IDocumentBusinessService
    {
        Task<Document> GetDocumentByIdAsync(string id);
        Task<IEnumerable<Document>> GetAllDocumentsAsync();
        Task<bool> CreateDocumentAsync(Document document);
        Task<bool> UpdateDocumentAsync(Document document);
        Task<bool> DeleteDocumentAsync(string id);
        Task<IEnumerable<Document>> SearchSimilarDocumentsAsync(string query, int limit = 5);
    }
}