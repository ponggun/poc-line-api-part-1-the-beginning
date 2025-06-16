using PocLineAPI.Domain.Entities;

namespace PocLineAPI.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Line events.
    /// </summary>
    public interface ILineEventRepository
    {
        Task<bool> AddAsync(LineEvent lineEvent);
        Task<LineEvent?> GetByIdAsync(string id);
        Task<IEnumerable<LineEvent>> GetAllAsync();
        Task<IEnumerable<LineEvent>> GetByTypeAsync(string type);
        Task<bool> DeleteAsync(string id);
    }
}