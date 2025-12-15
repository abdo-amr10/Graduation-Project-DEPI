using System.ComponentModel.DataAnnotations;

namespace My_Uni_Hub.Models.Pages
{
    public class LostItem
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";

        public string? Description { get; set; } = "";
        public string? Location { get; set; }
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public string Status { get; set; } = "lost";
        public int? StudentId { get; set; }
        public Student? Student { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public string? ContactInfo { get; set; }
        public bool IsFound { get; set; } = false;
        public string? OwnerName { get; set; }

    }
}
