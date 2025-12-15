using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace My_Uni_Hub.Models.Pages
{
    public class Student
    {

        public int Id { get; set; }          
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public int? AcademicYear { get; set; }   
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public bool IsVerified { get; set; } = false;
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public int? FacultyId { get; set; }
        public Faculty? Faculty { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Bio { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserId { get; set; }   // FK -> AspNetUsers.Id (Identity)
        public List<CourseStudent> CourseStudents { get; set; } = new();
        [NotMapped]
        public List<Material> Documents { get; set; } = new();

    }
}
