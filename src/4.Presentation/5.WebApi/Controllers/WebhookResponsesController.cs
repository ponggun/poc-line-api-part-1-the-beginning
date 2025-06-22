using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application.Services;
using PocLineAPI.Domain;

namespace PocLineAPI.Presentation.WebApi;

[ApiController]
[Route("api/[controller]")]
public class WebhookResponsesController : ControllerBase
{
    private readonly IWebhookResponseBusinessService _service;

    public WebhookResponsesController(IWebhookResponseBusinessService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WebhookResponse>>> GetAll()
    {
        var responses = await _service.GetAllAsync();
        return Ok(responses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WebhookResponse>> GetById(Guid id)
    {
        var response = await _service.GetByIdAsync(id);
        if (response == null)
            return NotFound();
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] WebhookResponse webhookResponse)
    {
        await _service.AddAsync(webhookResponse);
        return CreatedAtAction(nameof(GetById), new { id = webhookResponse.Id }, webhookResponse);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] WebhookResponse webhookResponse)
    {
        if (id != webhookResponse.Id)
            return BadRequest();
        if (!await _service.ExistsAsync(id))
            return NotFound();
        await _service.UpdateAsync(webhookResponse);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        if (!await _service.ExistsAsync(id))
            return NotFound();
        await _service.DeleteAsync(id);
        return NoContent();
    }
}