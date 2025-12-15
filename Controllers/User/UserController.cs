using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;
using System.Security.Claims;

namespace My_Uni_Hub.Controllers.User
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IStudentService _students;
        private readonly IMaterialService _materials;
        private readonly IWebHostEnvironment _env; 


        public UserController(IStudentService students, IMaterialService materials , IWebHostEnvironment env)
        {
            _students = students;
            _materials = materials;
            _env = env; 
        }

        [HttpGet("/user/profile")]
        public async Task<IActionResult> UserProfile(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var student = await _students.GetByUserIdAsync(userId, ct);

            if (student == null)
                return Content("No student profile found for this user.");

            var courseIds = student.CourseStudents
                                   .Select(cs => cs.CoursesId)
                                   .Distinct()
                                   .ToList();

            var allMaterials = new List<Material>();
            foreach (var courseId in courseIds)
            {
                var mats = await _materials.GetByCourseAsync(courseId, ct);
                if (mats != null && mats.Any())
                    allMaterials.AddRange(mats);
            }

            student.Documents = allMaterials
                                .OrderByDescending(m => m.UploadDate)
                                .ToList();


            return View(student);
        }

        [HttpGet("/user/profile/edit")]
        public async Task<IActionResult> EditProfilePartial(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var student = await _students.GetByUserIdAsync(userId, ct);
            if (student == null) return NotFound();

            var vm = new My_Uni_Hub.Models.ViewModels.UserViewModel.StudentEditViewModel
            {
                Id = student.Id,
                FullName = student.FullName,
                PhoneNumber = student.PhoneNumber,
                PhotoUrl = student.PhotoUrl
            };

            return PartialView("_EditProfilePartial", vm); 
        }

        [HttpPost("/user/profile/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(IFormFile? photo, [FromForm] My_Uni_Hub.Models.ViewModels.UserViewModel.StudentEditViewModel model, CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var student = await _students.GetByUserIdAsync(userId, ct);
            if (student == null) return NotFound();

            student.FullName = model.FullName?.Trim() ?? student.FullName;
            student.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Trim();

            if (photo != null && photo.Length > 0)
            {
                const long maxBytes = 10 * 1024 * 1024;
                if (photo.Length > maxBytes)
                    return BadRequest(new { success = false, message = "File too large (max 4MB)." });

                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                    return BadRequest(new { success = false, message = "Invalid image type. Allowed: jpg, jpeg, png, gif." });

                var webRoot = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploads = Path.Combine(webRoot, "uploads", "profiles");

                try
                {
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = "Cannot create uploads folder.", detail = ex.Message });
                }

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploads, fileName);

                try
                {
                    using (var fs = System.IO.File.Create(filePath))
                    {
                        await photo.CopyToAsync(fs, ct);
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = "Failed saving file.", detail = ex.Message });
                }

                student.PhotoUrl = $"/uploads/profiles/{fileName}";
            }

            await _students.UpdateAsync(student, ct);

            return  RedirectToAction(nameof(UserProfile) );
        }
    }
}
