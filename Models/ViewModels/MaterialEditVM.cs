using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace My_Uni_Hub.Models.ViewModels
{
    public class MaterialEditVM
    {
        [Required] public int Id { get; set; }

        [Required] public string Title { get; set; } = "";
        [Required] public string Description { get; set; } = "";

        public string? Category { get; set; }
        public int? CourseId { get; set; }

        // existing file path so we can show a link and delete/replace if uploaded
        public string? ExistingFilePath { get; set; }
        public string? ExistingFileName { get; set; }

        // NEW: file upload bound to this VM (optional)
        // Note: IFormFile is only used for binding the posted file — it is not persisted in DB.
        public IFormFile? File { get; set; }
    }
}
