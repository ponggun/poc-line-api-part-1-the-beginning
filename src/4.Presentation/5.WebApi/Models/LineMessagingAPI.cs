namespace PocLineAPI.Presentation.WebApi.Models
{
    public class LineMessagingAPI
    {
        public class WebhookRequest
        {
            public string? Destination { get; set; }
            public List<Event>? Events { get; set; }

            public class Event
            {
                public string? Type { get; set; }
                public Message? Message { get; set; }
                public string? WebhookEventId { get; set; }
                public DeliveryContextObject? DeliveryContext { get; set; }
                public long? Timestamp { get; set; }
                public SourceObject? Source { get; set; }
                public string? ReplyToken { get; set; }
                public string? Mode { get; set; }
                public Postback? Postback { get; set; }
            }

            public class Message
            {
                public string? Type { get; set; }
                public string? Id { get; set; }
                public string? QuoteToken { get; set; }
                public string? Text { get; set; }
            }

            public class DeliveryContextObject
            {
                public bool IsRedelivery { get; set; }
            }

            public class SourceObject
            {
                public string? Type { get; set; }
                public string? UserId { get; set; }
            }

            public class Postback
            {
                public string? Data { get; set; }
            }
        }
        public class TokenResponse
        {
            public string? access_token { get; set; }
        }
    }
}
