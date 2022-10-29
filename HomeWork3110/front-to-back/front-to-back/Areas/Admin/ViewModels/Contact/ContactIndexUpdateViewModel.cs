using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace front_to_back.Areas.Admin.ViewModels.Contact
{
    public class ContactIndexUpdateViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Title2 { get; set; }
        public string? Information { get; set; }
        public string? PhotoPath { get; set; }
        
     
        public IFormFile? Photo { get; set; }
    }
}
