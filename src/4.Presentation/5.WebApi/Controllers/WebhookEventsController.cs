using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application;
using PocLineAPI.Domain;

namespace PocLineAPI.Presentation.WebApi;

[ApiController]
[Route("api/[controller]")]
public class WebhookEventsController : ControllerBase
{
    private readonly IWebhookEventBusinessService _service;
    private readonly IErrorLogBusinessService _errorLogBusinessService;

    public WebhookEventsController(IWebhookEventBusinessService service, IErrorLogBusinessService ErrorLogBusinessService)
    {
        _service = service;
        _errorLogBusinessService = ErrorLogBusinessService;
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
            var errorCode = _errorLogBusinessService.LogUnexpectedError(ex);
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
            var errorCode = _errorLogBusinessService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }
}