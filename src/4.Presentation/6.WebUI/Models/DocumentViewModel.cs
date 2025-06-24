using System.ComponentModel.DataAnnotations;

namespace PocLineAPI.Presentation.WebUI.Models
{
    public class DocumentViewModel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Content is required")]
        [Display(Name = "Document Content")]
        public string Content { get; set; } = string.Empty;
    }

    public class DocumentListViewModel
    {
        public List<DocumentViewModel> Documents { get; set; } = new List<DocumentViewModel>();
    }
}