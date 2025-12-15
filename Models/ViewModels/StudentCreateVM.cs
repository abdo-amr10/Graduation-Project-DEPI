using System.ComponentModel.DataAnnotations;

namespace My_Uni_Hub.Models.ViewModels
{
    public class StudentCreateVM
    {
        [Required]
        public string FullName { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";
        public int? AcademicYear { get; set; }
        public int? DepartmentId { get; set; }
        public int? FacultyId { get; set; }
        public string? Bio { get; set; }
        public bool IsVerified { get; set; } = false;
    }
}
