using System.ComponentModel.DataAnnotations;

namespace My_Uni_Hub.Models.ViewModels.UserViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }
        
        public IFormFile? ProfileImage { get; set; }
        public string SelectedRole { get; set; }

        public int? DepartmentId { get; set; }      // القيمة المرسلة: Id لكن المستخدم يشوف الاسم
        public int? FacultyId { get; set; }         // نفس الشيء للكلية
        public int? AcademicYear { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
