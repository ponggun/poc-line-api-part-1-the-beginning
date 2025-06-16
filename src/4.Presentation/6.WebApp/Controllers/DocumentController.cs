using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Domain.Entities;
using PocLineAPI.Presentation.WebApp.Models;
using PocLineAPI.Presentation.WebApp.Services;

namespace PocLineAPI.Presentation.WebApp.Controllers
{
    public class DocumentController : Controller
    {
        private readonly DocumentApiClient _documentApiClient;

        public DocumentController(DocumentApiClient documentApiClient)
        {
            _documentApiClient = documentApiClient ?? throw new ArgumentNullException(nameof(documentApiClient));
        }

        public async Task<IActionResult> Index()
        {
            var documents = await _documentApiClient.GetAllDocumentsAsync();
            return View(documents);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Document document, string[] MetadataKeys, string[] MetadataValues)
        {
            if (ModelState.IsValid)
            {
                // Process metadata from form
                document.Metadata = new Dictionary<string, string>();
                for (int i = 0; i < MetadataKeys.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(MetadataKeys[i]) && i < MetadataValues.Length)
                    {
                        document.Metadata[MetadataKeys[i]] = MetadataValues[i];
                    }
                }
                
                await _documentApiClient.CreateDocumentAsync(document);
                return RedirectToAction(nameof(Index));
            }
            return View(document);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var document = await _documentApiClient.GetDocumentByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Document document, string[] MetadataKeys, string[] MetadataValues)
        {
            if (id != document.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Process metadata from form
                document.Metadata = new Dictionary<string, string>();
                for (int i = 0; i < MetadataKeys.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(MetadataKeys[i]) && i < MetadataValues.Length)
                    {
                        document.Metadata[MetadataKeys[i]] = MetadataValues[i];
                    }
                }
                
                await _documentApiClient.UpdateDocumentAsync(document);
                return RedirectToAction(nameof(Index));
            }
            return View(document);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var document = await _documentApiClient.GetDocumentByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _documentApiClient.DeleteDocumentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(new List<Document>());
            }

            var results = await _documentApiClient.SearchSimilarDocumentsAsync(query);
            return View(results);
        }
    }
}