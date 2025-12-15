using My_Uni_Hub.Models.Pages;

namespace My_Uni_Hub.Models.ViewModels.UserViewModel
{
    public class DashboardViewModel
    {
        // student summary
        public int StudentId { get; set; }
        public string FullName { get; set; } = "";
        public string? PhotoUrl { get; set; }
        public string? FacultyName { get; set; }
        public int? AcademicYear { get; set; }

        // courses
        public List<CourseCardVm> Courses { get; set; } = new();

        // announcements / feed
        public List<AnnouncementVm> Announcements { get; set; } = new();

        // optional: calendar/deadlines
        public List<SimpleEventVm> Events { get; set; } = new();

        public List<Opportunity> Opportunities { get; set; } = new();

    }

    public class CourseCardVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Lecturer { get; set; } = "";
        public string? Icon { get; set; }
        public string? ShortDesc { get; set; }
    }

    public class AnnouncementVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class SimpleEventVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public DateTime Date { get; set; }
        public bool IsDeadline { get; set; }
    }
}
