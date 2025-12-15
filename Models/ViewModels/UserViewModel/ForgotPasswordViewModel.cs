using System.ComponentModel.DataAnnotations;

namespace My_Uni_Hub.Models.ViewModels.UserViewModel
{
    public class ForgotPasswordViewModel
    {

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
