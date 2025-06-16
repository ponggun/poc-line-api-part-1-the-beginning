namespace PocLineAPI.Domain.Entities
{
    /// <summary>
    /// Represents a Line webhook event received from Line Messaging API.
    /// </summary>
    public class LineEvent
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Source { get; set; } = string.Empty;
        public string ReplyToken { get; set; } = string.Empty;
        public Dictionary<string, object> RawData { get; set; } = new Dictionary<string, object>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public LineEvent()
        {
        }

        public LineEvent(string type, string source, string replyToken, Dictionary<string, object>? rawData = null)
        {
            Id = Guid.NewGuid().ToString();
            Type = type;
            Source = source;
            ReplyToken = replyToken;
            Timestamp = DateTime.UtcNow;
            RawData = rawData ?? new Dictionary<string, object>();
            CreatedAt = DateTime.UtcNow;
        }
    }
}