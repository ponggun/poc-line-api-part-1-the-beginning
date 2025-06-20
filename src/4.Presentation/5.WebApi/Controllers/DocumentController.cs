using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application;
using PocLineAPI.Domain;

namespace PocLineAPI.Presentation.WebApi;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentBusinessService _DocumentBusinessService;
    private readonly ILogger<DocumentController> _logger;

    public DocumentController(IDocumentBusinessService DocumentBusinessService, ILogger<DocumentController> logger)
    {
        _DocumentBusinessService = DocumentBusinessService ?? throw new ArgumentNullException(nameof(DocumentBusinessService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Document>>> GetAllDocuments()
    {
        _logger.LogInformation("Getting all documents");
        var documents = await _DocumentBusinessService.GetAllDocumentsAsync();
        return Ok(documents);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Document>> GetDocument(string id)
    {
        _logger.LogInformation("Getting document with id: {Id}", id);
        var document = await _DocumentBusinessService.GetDocumentByIdAsync(id);
        if (document == null)
        {
            _logger.LogWarning("Document with id {Id} not found", id);
            return NotFound();
        }
        return Ok(document);
    }

    [HttpPost]
    public async Task<ActionResult<Document>> CreateDocument([FromBody] Document document)
    {
        if (document == null)
        {
            _logger.LogWarning("CreateDocument called with null document");
            return BadRequest();
        }

        var success = await _DocumentBusinessService.CreateDocumentAsync(document);
        if (success)
        {
            _logger.LogInformation("Document created with id: {Id}", document.Id);
            return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, document);
        }
        _logger.LogWarning("Failed to create document");
        return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDocument(string id, [FromBody] Document document)
    {
        if (document == null || id != document.Id)
        {
            _logger.LogWarning("UpdateDocument called with invalid data. Id: {Id}, DocumentId: {DocumentId}", id, document?.Id);
            return BadRequest();
        }

        var success = await _DocumentBusinessService.UpdateDocumentAsync(document);
        if (success)
        {
            _logger.LogInformation("Document updated with id: {Id}", id);
            return NoContent();
        }
        _logger.LogWarning("Document with id {Id} not found for update", id);
        return NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDocument(string id)
    {
        var success = await _DocumentBusinessService.DeleteDocumentAsync(id);
        if (success)
        {
            _logger.LogInformation("Document deleted with id: {Id}", id);
            return NoContent();
        }
        _logger.LogWarning("Document with id {Id} not found for deletion", id);
        return NotFound();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Document>>> SearchDocuments([FromQuery] string query, [FromQuery] int limit = 5)
    {
        _logger.LogInformation("Searching documents with query: {Query}, limit: {Limit}", query, limit);
        var documents = await _DocumentBusinessService.SearchSimilarDocumentsAsync(query, limit);
        return Ok(documents);
    }
}