using System.ComponentModel.DataAnnotations;

namespace My_Uni_Hub.Models.Pages
{
    public class Opportunity
    {
        [Key]
        public int OpportunityId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Type { get; set; } // Internship - Job - Course
        public string? Organization { get; set; }
        public string Description { get; set; }
        public string ApplyLink { get; set; }
        public DateTime PostedAt { get; set; } = DateTime.Now;

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? StudentId { get; set; }
        public Student? Student { get; set; }
        public int? FacultyId { get; set; }
        public Faculty? Faculty { get; set; }

    }
}
