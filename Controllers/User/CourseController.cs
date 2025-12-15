using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Controllers.User
{
    [Authorize]
    [Route("user/courses")]
    public class CourseController : Controller
    {
        private readonly MyUniDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorageService _files;

        public CourseController(MyUniDbContext db, UserManager<ApplicationUser> userManager, IFileStorageService files)
        {
            _db = db;
            _userManager = userManager;
            _files = files;
        }

        // GET /courses/my
        [HttpGet("")]
        public async Task<IActionResult> MyCourses(CancellationToken ct)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return Challenge();

            var studentId = await GetStudentIdForCurrentUser(identityUser.Id, ct);
            if (studentId == null)
            {
                TempData["Error"] = "لا يمكن معرفة بيانات الطالب المرتبطة بحسابك. تواصل مع الأدمن.";
                return View("UserCourses", new List<Course>());
            }

            var courses = await _db.Courses
                .Where(c => c.CourseStudents.Any(cs => cs.StudentsId == studentId.Value))
                .Include(c => c.Materials)
                .Include(c => c.Faculty)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync(ct);

            return View("UserCourses", courses);
        }

        // GET /courses/download/{id}
        [HttpGet("download/{id:int}")]
        public async Task<IActionResult> Download(int id, CancellationToken ct)
        {
            var mat = await _db.Materials.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);
            if (mat == null) return NotFound();

            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return Challenge();

            var studentId = await GetStudentIdForCurrentUser(identityUser.Id, ct);
            if (studentId == null) return Forbid();

            var isEnrolled = await _db.CourseStudents
                .AnyAsync(cs => cs.CoursesId == mat.CourseId && cs.StudentsId == studentId.Value, ct);

            if (!isEnrolled && !User.IsInRole("Admin"))
                return Forbid();

            if (!string.IsNullOrWhiteSpace(mat.FilePath) &&
                (mat.FilePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                 mat.FilePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            {
                return Redirect(mat.FilePath);
            }

            if (!string.IsNullOrWhiteSpace(mat.FilePath) && mat.FilePath.StartsWith("/"))
            {
                return Redirect(mat.FilePath);
            }

            if (!string.IsNullOrWhiteSpace(mat.FilePath))
            {
                var rel = mat.FilePath.TrimStart('/');
                var physical = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rel.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(physical))
                {
                    var mem = new MemoryStream();
                    using (var fs = new FileStream(physical, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await fs.CopyToAsync(mem, ct);
                    }
                    mem.Position = 0;
                    var contentType = GetContentType(mat);
                    return File(mem, contentType ?? "application/octet-stream", mat.Title);
                }
            }

            return NotFound();
        }


        private async Task<int?> GetStudentIdForCurrentUser(string identityUserId, CancellationToken ct)
        {
            var studentByUserId = await _db.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == identityUserId, ct);

            if (studentByUserId != null)
                return studentByUserId.Id;

            if (int.TryParse(identityUserId, out var parsed))
                return parsed;

            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser != null)
            {
                var studentByEmail = await _db.Students
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Email == identityUser.Email, ct);

                if (studentByEmail != null)
                    return studentByEmail.Id;
            }

            return null;
        }

        private string? GetContentType(Material mat)
        {
            var ext = mat.FileType;
            if (string.IsNullOrWhiteSpace(ext))
                ext = Path.GetExtension(mat.Title ?? "");

            if (string.IsNullOrWhiteSpace(ext)) return null;
            ext = ext.StartsWith(".") ? ext.ToLowerInvariant() : "." + ext.ToLowerInvariant();

            return ext switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".zip" => "application/zip",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".mp4" => "video/mp4",
                _ => null
            };
        }
    }
}
