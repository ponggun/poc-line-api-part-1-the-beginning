using PocLineAPI.Domain.Entities;

namespace PocLineAPI.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Line messages.
    /// </summary>
    public interface ILineMessageRepository
    {
        Task<bool> AddAsync(LineMessage lineMessage);
        Task<LineMessage?> GetByIdAsync(string id);
        Task<IEnumerable<LineMessage>> GetAllAsync();
        Task<IEnumerable<LineMessage>> GetByEventIdAsync(string eventId);
        Task<IEnumerable<LineMessage>> GetByUserIdAsync(string userId);
        Task<bool> DeleteAsync(string id);
    }
}