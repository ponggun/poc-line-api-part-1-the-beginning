using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application;
using PocLineAPI.Domain;

namespace PocLineAPI.Presentation.WebApi;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentBusinessService _documentBusinessService;

    public DocumentController(IDocumentBusinessService documentBusinessService)
    {
        _documentBusinessService = documentBusinessService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var doc = await _documentBusinessService.GetByIdAsync(id);
        if (doc == null)
        {
            return NotFound();
        }
        return Ok(doc);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var docs = await _documentBusinessService.GetAllAsync();
        return Ok(docs);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Document document)
    {
        if (document == null)
        {
            return BadRequest("Document is null.");
        }

        await _documentBusinessService.AddAsync(document);
        return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Document document)
    {
        if (document == null || id != document.Id)
        {
            return BadRequest("Invalid document data.");
        }
        if (!await _documentBusinessService.ExistsAsync(id))
        {
            return NotFound();
        }
        await _documentBusinessService.UpdateAsync(document);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await _documentBusinessService.ExistsAsync(id))
        {
            return NotFound();
        }
        await _documentBusinessService.DeleteAsync(id);
        return NoContent();
    }
}
