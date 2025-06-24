using Microsoft.AspNetCore.Mvc;
using PocLineAPI.Presentation.WebUI.Models;
using PocLineAPI.Presentation.WebUI.Services;

namespace PocLineAPI.Presentation.WebUI.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentApiService _documentApiService;

        public DocumentController(IDocumentApiService documentApiService)
        {
            _documentApiService = documentApiService;
        }

        // GET: Document
        public async Task<IActionResult> Index()
        {
            var documents = await _documentApiService.GetAllAsync();
            var viewModel = new DocumentListViewModel
            {
                Documents = documents
            };
            return View(viewModel);
        }

        // GET: Document/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var document = await _documentApiService.GetByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return View(document);
        }

        // GET: Document/Create
        public IActionResult Create()
        {
            return View(new DocumentViewModel());
        }

        // POST: Document/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentViewModel document)
        {
            if (ModelState.IsValid)
            {
                // Generate new ID for create
                document.Id = Guid.NewGuid();
                
                var success = await _documentApiService.CreateAsync(document);
                if (success)
                {
                    TempData["SuccessMessage"] = "Document created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create document. Please try again.";
                }
            }
            return View(document);
        }

        // GET: Document/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var document = await _documentApiService.GetByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return View(document);
        }

        // POST: Document/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DocumentViewModel document)
        {
            if (id != document.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var success = await _documentApiService.UpdateAsync(document);
                if (success)
                {
                    TempData["SuccessMessage"] = "Document updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update document. Please try again.";
                }
            }
            return View(document);
        }

        // GET: Document/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var document = await _documentApiService.GetByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return View(document);
        }

        // POST: Document/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var success = await _documentApiService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Document deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete document. Please try again.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}