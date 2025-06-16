using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application.Interfaces;
using PocLineAPI.Domain.Entities;

namespace PocLineAPI.Presentation.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Document>>> GetAllDocuments()
        {
            var documents = await _documentService.GetAllDocumentsAsync();
            return Ok(documents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetDocument(string id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return Ok(document);
        }

        [HttpPost]
        public async Task<ActionResult<Document>> CreateDocument([FromBody] Document document)
        {
            if (document == null)
            {
                return BadRequest();
            }

            var success = await _documentService.CreateDocumentAsync(document);
            if (success)
            {
                return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, document);
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(string id, [FromBody] Document document)
        {
            if (document == null || id != document.Id)
            {
                return BadRequest();
            }

            var success = await _documentService.UpdateDocumentAsync(document);
            if (success)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(string id)
        {
            var success = await _documentService.DeleteDocumentAsync(id);
            if (success)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Document>>> SearchDocuments([FromQuery] string query, [FromQuery] int limit = 5)
        {
            var documents = await _documentService.SearchSimilarDocumentsAsync(query, limit);
            return Ok(documents);
        }
    }
}