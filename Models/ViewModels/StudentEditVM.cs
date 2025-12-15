namespace My_Uni_Hub.Models.ViewModels
{
    public class StudentEditVM : StudentCreateVM
    {
        public int Id { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; } // added so view can show Joined At


    }
}
