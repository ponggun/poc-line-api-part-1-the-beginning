using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application;
using PocLineAPI.Domain;

namespace PocLineAPI.Presentation.WebApi;

[ApiController]
[Route("api/[controller]")]
public class WebhookEventsController : ControllerBase
{
    private readonly IWebhookEventBusinessService _service;
    private readonly IErrorLogService _errorLogService;

    public WebhookEventsController(IWebhookEventBusinessService service, IErrorLogService errorLogService)
    {
        _service = service;
        _errorLogService = errorLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WebhookEvent>>> GetAll()
    {
        try
        {
            var events = await _service.GetAllAsync();
            return Ok(events);
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WebhookEvent>> GetById(Guid id)
    {
        try
        {
            var webhookEvent = await _service.GetByIdAsync(id);
            if (webhookEvent == null)
                return NotFound();
            return Ok(webhookEvent);
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] WebhookEvent webhookEvent)
    {
        try
        {
            var created = await _service.AddAsync(webhookEvent);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] WebhookEvent webhookEvent)
    {
        if (id != webhookEvent.Id)
            return BadRequest();
        try
        {
            if (!await _service.ExistsAsync(id))
                return NotFound();
            var updated = await _service.UpdateAsync(webhookEvent);
            if (!updated) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            if (!await _service.ExistsAsync(id))
                return NotFound();
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }
}