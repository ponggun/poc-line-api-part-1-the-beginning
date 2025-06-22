using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application;
using PocLineAPI.Domain;

namespace PocLineAPI.Presentation.WebApi;

[ApiController]
[Route("api/[controller]")]
public class WebhookResponsesController : ControllerBase
{
    private readonly IWebhookResponseBusinessService _service;
    private readonly IErrorLogBusinessService _errorLogBusinessService;

    public WebhookResponsesController(IWebhookResponseBusinessService service, IErrorLogBusinessService ErrorLogBusinessService)
    {
        _service = service;
        _errorLogBusinessService = ErrorLogBusinessService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WebhookResponse>>> GetAll()
    {
        try
        {
            var responses = await _service.GetAllAsync();
            return Ok(responses);
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogBusinessService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WebhookResponse>> GetById(Guid id)
    {
        try
        {
            var response = await _service.GetByIdAsync(id);
            if (response == null)
                return NotFound();
            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogBusinessService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }
}