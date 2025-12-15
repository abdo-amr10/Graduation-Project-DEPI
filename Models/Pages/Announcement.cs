using System.Globalization;

namespace My_Uni_Hub.Models.Pages
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";      
        public int? FacultyId { get; set; }           
        public Faculty? Faculty { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
