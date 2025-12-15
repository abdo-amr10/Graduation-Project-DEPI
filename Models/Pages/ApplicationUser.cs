using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace My_Uni_Hub.Models.Pages
{

        public class ApplicationUser : IdentityUser
        {
            [Required]
            [StringLength(100)]
            public string FullName { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public DateTime? LastLogin { get; set; }

            public bool IsActive { get; set; } = true;

            [StringLength(500)]
            public string? Address { get; set; }

            public DateTime? DateOfBirth { get; set; }
            public string ProfileImagePath { get; set; }
    }
}
