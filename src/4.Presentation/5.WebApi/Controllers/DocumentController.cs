using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application;
using PocLineAPI.Domain;

namespace PocLineAPI.Presentation.WebApi;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly ILogger<DocumentController> _logger;
    private readonly IDocumentBusinessService _documentBusinessService;

    public DocumentController(ILogger<DocumentController> logger, IDocumentBusinessService documentBusinessService)
    {
        _logger = logger;
        _documentBusinessService = documentBusinessService;

        // Example usage of logger in the controller constructor
        _logger.LogInformation("DocumentController initialized");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation($"Getting document with id: {id}");
        var doc = await _documentBusinessService.GetByIdAsync(id);
        if (doc == null)
        {
            _logger.LogWarning($"Document with id {id} not found.");
            return NotFound();
        }
        return Ok(doc);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Getting all documents");
        var docs = await _documentBusinessService.GetAllAsync();
        return Ok(docs);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Document document)
    {
        if (document == null)
        {
            _logger.LogWarning("Attempted to add null document");
            return BadRequest("Document is null.");
        }
        document.Id = Guid.NewGuid();
        await _documentBusinessService.AddAsync(document);
        _logger.LogInformation($"Added document with id: {document.Id}");
        return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Document document)
    {
        if (document == null || id != document.Id)
        {
            _logger.LogWarning($"Invalid update attempt for document id: {id}");
            return BadRequest("Invalid document data.");
        }
        var existing = await _documentBusinessService.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning($"Document with id {id} not found for update.");
            return NotFound();
        }
        await _documentBusinessService.UpdateAsync(document);
        _logger.LogInformation($"Updated document with id: {id}");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _documentBusinessService.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning($"Document with id {id} not found for delete.");
            return NotFound();
        }
        await _documentBusinessService.DeleteAsync(id);
        _logger.LogInformation($"Deleted document with id: {id}");
        return NoContent();
    }
}
