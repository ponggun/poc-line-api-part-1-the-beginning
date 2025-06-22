using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application.Services;
using PocLineAPI.Domain;

namespace PocLineAPI.Presentation.WebApi;

[ApiController]
[Route("api/[controller]")]
public class WebhookEventsController : ControllerBase
{
    private readonly IWebhookEventBusinessService _service;

    public WebhookEventsController(IWebhookEventBusinessService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WebhookEvent>>> GetAll()
    {
        var events = await _service.GetAllAsync();
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WebhookEvent>> GetById(Guid id)
    {
        var webhookEvent = await _service.GetByIdAsync(id);
        if (webhookEvent == null)
            return NotFound();
        return Ok(webhookEvent);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] WebhookEvent webhookEvent)
    {
        var created = await _service.AddAsync(webhookEvent);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] WebhookEvent webhookEvent)
    {
        if (id != webhookEvent.Id)
            return BadRequest();
        if (!await _service.ExistsAsync(id))
            return NotFound();
        var updated = await _service.UpdateAsync(webhookEvent);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        if (!await _service.ExistsAsync(id))
            return NotFound();
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}