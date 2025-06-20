using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Application.Interfaces;
using PocLineAPI.Domain.Entities;

namespace PocLineAPI.Presentation.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentBusinessService _DocumentBusinessService;

        public DocumentController(IDocumentBusinessService DocumentBusinessService)
        {
            _DocumentBusinessService = DocumentBusinessService ?? throw new ArgumentNullException(nameof(DocumentBusinessService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Document>>> GetAllDocuments()
        {
            var documents = await _DocumentBusinessService.GetAllDocumentsAsync();
            return Ok(documents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetDocument(string id)
        {
            var document = await _DocumentBusinessService.GetDocumentByIdAsync(id);
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

            var success = await _DocumentBusinessService.CreateDocumentAsync(document);
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

            var success = await _DocumentBusinessService.UpdateDocumentAsync(document);
            if (success)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(string id)
        {
            var success = await _DocumentBusinessService.DeleteDocumentAsync(id);
            if (success)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Document>>> SearchDocuments([FromQuery] string query, [FromQuery] int limit = 5)
        {
            var documents = await _DocumentBusinessService.SearchSimilarDocumentsAsync(query, limit);
            return Ok(documents);
        }
    }
}