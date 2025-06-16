using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Domain.Entities;
using PocLineAPI.Domain.Interfaces;
using Serilog;
using System.Text.Json;

namespace PocLineAPI.Presentation.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineWebhookController : ControllerBase
    {
        private readonly ILineEventRepository _lineEventRepository;
        private readonly ILineMessageRepository _lineMessageRepository;
        private readonly ILogger<LineWebhookController> _logger;

        public LineWebhookController(
            ILineEventRepository lineEventRepository,
            ILineMessageRepository lineMessageRepository,
            ILogger<LineWebhookController> logger)
        {
            _lineEventRepository = lineEventRepository ?? throw new ArgumentNullException(nameof(lineEventRepository));
            _lineMessageRepository = lineMessageRepository ?? throw new ArgumentNullException(nameof(lineMessageRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] JsonElement payload)
        {
            try
            {
                _logger.LogInformation("Received Line webhook payload: {Payload}", payload.GetRawText());
                
                // Extract events from the payload
                if (payload.TryGetProperty("events", out var eventsElement) && eventsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var eventElement in eventsElement.EnumerateArray())
                    {
                        await ProcessLineEvent(eventElement);
                    }
                }
                else
                {
                    _logger.LogWarning("No events found in payload");
                }

                return Ok(new { status = "success", message = "Webhook processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Line webhook");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        private async Task ProcessLineEvent(JsonElement eventElement)
        {
            try
            {
                // Extract basic event properties
                var eventType = eventElement.TryGetProperty("type", out var typeProperty) ? typeProperty.GetString() ?? string.Empty : string.Empty;
                var replyToken = eventElement.TryGetProperty("replyToken", out var replyTokenProperty) ? replyTokenProperty.GetString() ?? string.Empty : string.Empty;
                var timestampMs = eventElement.TryGetProperty("timestamp", out var timestampProperty) ? timestampProperty.GetInt64() : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                
                // Extract source information
                var sourceJson = eventElement.TryGetProperty("source", out var sourceProperty) ? sourceProperty.GetRawText() : "{}";
                
                // Convert raw JSON to dictionary for storage
                var rawData = JsonSerializer.Deserialize<Dictionary<string, object>>(eventElement.GetRawText()) ?? new Dictionary<string, object>();
                
                // Create and store Line event
                var lineEvent = new LineEvent(
                    type: eventType,
                    source: sourceJson,
                    replyToken: replyToken,
                    rawData: rawData
                );
                lineEvent.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).DateTime;

                var eventStored = await _lineEventRepository.AddAsync(lineEvent);
                _logger.LogInformation("Line event stored: {EventId}, Type: {EventType}, Success: {Success}", 
                    lineEvent.Id, eventType, eventStored);

                // Process message if event is a message type
                if (eventType == "message" && eventElement.TryGetProperty("message", out var messageElement))
                {
                    await ProcessLineMessage(lineEvent.Id, messageElement, eventElement);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing individual Line event");
            }
        }

        private async Task ProcessLineMessage(string eventId, JsonElement messageElement, JsonElement eventElement)
        {
            try
            {
                var messageId = messageElement.TryGetProperty("id", out var idProperty) ? idProperty.GetString() ?? string.Empty : string.Empty;
                var messageType = messageElement.TryGetProperty("type", out var typeProperty) ? typeProperty.GetString() ?? string.Empty : string.Empty;
                var text = messageElement.TryGetProperty("text", out var textProperty) ? textProperty.GetString() ?? string.Empty : string.Empty;
                
                // Extract user ID from source
                var userId = string.Empty;
                if (eventElement.TryGetProperty("source", out var sourceElement) && sourceElement.TryGetProperty("userId", out var userIdProperty))
                {
                    userId = userIdProperty.GetString() ?? string.Empty;
                }

                // Convert message metadata
                var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(messageElement.GetRawText()) ?? new Dictionary<string, object>();

                var lineMessage = new LineMessage(
                    eventId: eventId,
                    messageId: messageId,
                    type: messageType,
                    text: text,
                    userId: userId,
                    metadata: metadata
                );

                var messageStored = await _lineMessageRepository.AddAsync(lineMessage);
                _logger.LogInformation("Line message stored: {MessageId}, Text: {Text}, Success: {Success}", 
                    messageId, text, messageStored);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Line message");
            }
        }

        [HttpGet("events")]
        public async Task<ActionResult<IEnumerable<LineEvent>>> GetEvents()
        {
            try
            {
                var events = await _lineEventRepository.GetAllAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Line events");
                return StatusCode(500, new { status = "error", message = "Error retrieving events" });
            }
        }

        [HttpGet("messages")]
        public async Task<ActionResult<IEnumerable<LineMessage>>> GetMessages()
        {
            try
            {
                var messages = await _lineMessageRepository.GetAllAsync();
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Line messages");
                return StatusCode(500, new { status = "error", message = "Error retrieving messages" });
            }
        }
    }
}