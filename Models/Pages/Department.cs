namespace My_Uni_Hub.Models.Pages
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? FacultyId { get; set; }

        public Faculty Faculty { get; set; }

        public ICollection<Course> courses { get; set; } = new List<Course>();

        public ICollection<Student> students { get; set; } = new List<Student>();



    }
}
