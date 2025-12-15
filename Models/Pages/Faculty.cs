using System.ComponentModel.DataAnnotations;

namespace My_Uni_Hub.Models.Pages
{
    public class Faculty
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Department> departments { get; set; } = new List<Department>();
        public ICollection<Student> students { get; set; } = new List<Student>();
        public ICollection<Announcement> announcements { get; set; } = new List<Announcement>();

    }
}
