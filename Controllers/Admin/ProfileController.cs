using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.ViewModels;
using My_Uni_Hub.Services.Interfaces;
using System.Threading;

namespace My_Uni_Hub.Controllers.Admin
{
    [Route("admin/profile")]
    public class ProfileController : Controller
    {
        private readonly IStudentService _students;
        private readonly IFileStorageService _files;

        public ProfileController(IStudentService students, IFileStorageService files)
        {
            _students = students;
            _files = files;
        }

        // GET: /admin/profile/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var s = await _students.GetByIdAsync(id, ct);
            if (s == null) return NotFound();
            return View(s); 
        }

        // GET: /admin/profile/edit/{id}
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var s = await _students.GetByIdAsync(id, ct);
            if (s == null) return NotFound();

            var vm = new StudentEditVM
            {
                Id = s.Id,
                FullName = s.FullName,
                Email = s.Email,
                AcademicYear = s.AcademicYear,
                DepartmentId = s.DepartmentId,
                FacultyId = s.FacultyId,
                Bio = s.Bio,
                PhotoUrl = s.PhotoUrl
            };

            return View(vm);
        }

        // POST: /admin/profile/edit/{id}
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentEditVM vm, IFormFile? photo, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            var s = await _students.GetByIdAsync(vm.Id, ct);
            if (s == null) return NotFound();

            if (!string.Equals(s.Email, vm.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _students.EmailExistsAsync(vm.Email, ct))
                {
                    ModelState.AddModelError(nameof(vm.Email), "Email already exists.");
                    return View(vm);
                }
            }

            s.FullName = vm.FullName;
            s.Email = vm.Email;
            s.AcademicYear = vm.AcademicYear;
            s.DepartmentId = vm.DepartmentId;
            s.FacultyId = vm.FacultyId;
            s.Bio = vm.Bio;

            if (photo != null && photo.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(s.PhotoUrl))
                {
                    await _files.DeleteFileAsync(s.PhotoUrl, ct);
                }

                s.PhotoUrl = await _files.SaveFileAsync(photo, "profiles", ct);
            }

            await _students.UpdateAsync(s, ct);
            return RedirectToAction(nameof(Details), new { id = s.Id });
        }

        // POST: /admin/profile/delete/{id}
        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var s = await _students.GetByIdAsync(id, ct);
            if (s == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(s.PhotoUrl))
            {
                await _files.DeleteFileAsync(s.PhotoUrl, ct);
            }

            await _students.DeleteAsync(id, ct);
            return RedirectToAction("Index", "Users"); 
        }
    }
}
