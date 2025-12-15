using System.ComponentModel.DataAnnotations;

namespace My_Uni_Hub.Models.Pages
{
    public class Material
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string FilePath { get; set; } = "";
        public string? FileType { get; set; }
        public string? Category { get; set; }   
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public float Rating { get; set; }
        public string Description { get; set; } = "";
        public int? CourseId { get; set; }
        public Course? Course { get; set; }
    }
}
