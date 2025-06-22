using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PocLineAPI.Domain;

/// <summary>
/// Represents a log of a response or further processing related to a webhook event.
/// </summary>
public class WebhookResponse
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("WebhookEventId")]
    public Guid WebhookEventId { get; set; }
    public WebhookEvent? WebhookEvent { get; set; }

    [Required]
    public required string Response { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTimeOffset? RespondedAt { get; set; }
}

public interface IWebhookResponseRepository
{
    Task<WebhookResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<WebhookResponse>> GetAllAsync();
    Task AddAsync(WebhookResponse webhookResponse);
    Task UpdateAsync(WebhookResponse webhookResponse);
    Task DeleteAsync(Guid id);
}

