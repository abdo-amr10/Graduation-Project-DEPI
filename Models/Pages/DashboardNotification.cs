namespace My_Uni_Hub.Models.Pages
{
    public class DashboardNotification
    {
        
            public int Id { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public DateTime SentAt { get; set; } = DateTime.Now;
            public bool IsRead { get; set; } = false;

            public int? StudentId { get; set; }
            public Student Student { get; set; }
        
    }
}
