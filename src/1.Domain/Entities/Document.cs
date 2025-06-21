namespace PocLineAPI.Domain
{
    /// <summary>
    /// Represents a document that can be stored and retrieved from the vector database.
    /// </summary>
    public class Document
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
    }

    public interface IDocumentRepository
    {
        Task<Document?> GetByIdAsync(Guid id);
        Task<IEnumerable<Document>> GetAllAsync();
        Task AddAsync(Document document);
        Task UpdateAsync(Document document);
        Task DeleteAsync(Guid id);
    }

}