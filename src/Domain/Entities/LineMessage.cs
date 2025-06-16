namespace PocLineAPI.Domain.Entities
{
    /// <summary>
    /// Represents a Line message received from webhook.
    /// </summary>
    public class LineMessage
    {
        public string Id { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public string MessageId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public LineMessage()
        {
        }

        public LineMessage(string eventId, string messageId, string type, string text, string userId, Dictionary<string, object>? metadata = null)
        {
            Id = Guid.NewGuid().ToString();
            EventId = eventId;
            MessageId = messageId;
            Type = type;
            Text = text;
            UserId = userId;
            Timestamp = DateTime.UtcNow;
            Metadata = metadata ?? new Dictionary<string, object>();
            CreatedAt = DateTime.UtcNow;
        }
    }
}