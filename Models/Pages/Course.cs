using System.ComponentModel.DataAnnotations;

namespace My_Uni_Hub.Models.Pages
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseCode { get; set; }
        public string Semester { get; set; }
        public string Description { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        [Required]
        public int FacultyId { get; set; }
        public Faculty? Faculty { get; set; }


        public ICollection<Material> Materials { get; set; } = new List<Material>();
        public ICollection<ExamArchive> ExamArchives { get; set; } = new List<ExamArchive>();
        public ICollection<Question> questions { get; set; } = new List<Question>();
        public List<CourseStudent> CourseStudents { get; set; } = new();



    }
}
