namespace My_Uni_Hub.Models.ViewModels.UserViewModel
{
    public class StudentEditViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string? PhotoUrl { get; set; } 
    }
}
