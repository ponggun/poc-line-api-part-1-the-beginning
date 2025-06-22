using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application;
using PocLineAPI.Domain;

namespace PocLineAPI.Presentation.WebApi;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentBusinessService _documentBusinessService;
    private readonly IErrorLogBusinessService _errorLogBusinessService;

    public DocumentController(IDocumentBusinessService documentBusinessService, IErrorLogBusinessService ErrorLogBusinessService)
    {
        _documentBusinessService = documentBusinessService;
        _errorLogBusinessService = ErrorLogBusinessService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var doc = await _documentBusinessService.GetByIdAsync(id);
            if (doc == null)
            {
                return NotFound();
            }
            return Ok(doc);
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogBusinessService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var docs = await _documentBusinessService.GetAllAsync();
            return Ok(docs);
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogBusinessService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Document document)
    {
        if (document == null)
        {
            return BadRequest("Document is null.");
        }
        try
        {
            await _documentBusinessService.AddAsync(document);
            return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogBusinessService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Document document)
    {
        if (document == null || id != document.Id)
        {
            return BadRequest("Invalid document data.");
        }
        try
        {
            if (!await _documentBusinessService.ExistsAsync(id))
            {
                return NotFound();
            }
            await _documentBusinessService.UpdateAsync(document);
            return NoContent();
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogBusinessService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            if (!await _documentBusinessService.ExistsAsync(id))
            {
                return NotFound();
            }
            await _documentBusinessService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            var errorCode = _errorLogBusinessService.LogUnexpectedError(ex);
            return StatusCode(500, $"Internal server error. Error Code: {errorCode}");
        }
    }
}
