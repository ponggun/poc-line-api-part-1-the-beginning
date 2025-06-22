using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PocLineAPI.Domain
{
    /// <summary>
    /// Represents an event received from a webhook, stored for processing and auditing.
    /// </summary>
    public class WebhookEvent
    {
        [Key]
        public Guid Id { get; set; }
        
        [Column(TypeName = "jsonb")]
        public required JsonDocument EventJson { get; set; } // JSONB in DB

        public string? EventType { get; set; }
        public bool Processed { get; set; }
        public DateTimeOffset? ProcessedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public string? LineWebhookEventId { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }

    public interface IWebhookEventRepository
    {
        Task<WebhookEvent?> GetByIdAsync(Guid id);
        Task<IEnumerable<WebhookEvent>> GetAllAsync();
        Task AddAsync(WebhookEvent webhookEvent);
        Task UpdateAsync(WebhookEvent webhookEvent);
        Task DeleteAsync(Guid id);
    }
}
