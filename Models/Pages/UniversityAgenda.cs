using System.ComponentModel.DataAnnotations;

namespace My_Uni_Hub.Models.Pages
{
    public class UniversityAgenda
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? Type { get; set; }
        public int? FacultyId { get; set; }
        public Faculty? Faculty { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? StudentId { get; set; }
        public Student? Student { get; set; }

    }
}
