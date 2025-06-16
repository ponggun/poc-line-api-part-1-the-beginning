using PocLineAPI.Domain.Entities;

namespace PocLineAPI.Domain.Interfaces
{
    /// <summary>
    /// Defines methods for accessing and manipulating data in the repository.
    /// </summary>
    public interface IRepository
    {
        Task<Document> GetByIdAsync(string id);
        Task<IEnumerable<Document>> GetAllAsync();
        Task<bool> AddAsync(Document document);
        Task<bool> UpdateAsync(Document document);
        Task<bool> DeleteAsync(string id);
    }
}